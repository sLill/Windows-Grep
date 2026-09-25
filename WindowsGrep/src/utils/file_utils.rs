use std::cell::Cell;
use std::io::Read;
use std::panic::{self, UnwindSafe};
use std::sync::Once;

/// Reads text from a DOCX file by unzipping it and extracting `<w:t>` text nodes.
pub fn read_docx(filepath: &str) -> Result<String, String> {
    contain_panics(|| {
        let file = std::fs::File::open(filepath).map_err(|e| e.to_string())?;
        let mut archive = zip::ZipArchive::new(file).map_err(|e| e.to_string())?;

        let xml_content = {
            let mut doc = archive
                .by_name("word/document.xml")
                .map_err(|_| "word/document.xml not found in DOCX archive".to_string())?;
            let mut s = String::new();
            doc.read_to_string(&mut s).map_err(|e| e.to_string())?;
            s
        };

        extract_docx_text(&xml_content)
    })
}

fn extract_docx_text(xml: &str) -> Result<String, String> {
    use quick_xml::events::Event;
    use quick_xml::Reader;

    let mut reader = Reader::from_str(xml);
    reader.config_mut().trim_text(false);

    let mut output = String::new();
    let mut buf = Vec::new();
    let mut in_text_run = false;

    loop {
        match reader.read_event_into(&mut buf) {
            Ok(Event::Start(ref e)) => {
                let qname = e.name();
                let local = local_name(qname.as_ref());
                if local == b"t" {
                    in_text_run = true;
                } else if local == b"p" {
                    output.push('\n');
                }
            }
            Ok(Event::End(_)) => {
                in_text_run = false;
            }
            Ok(Event::Text(e)) => {
                if in_text_run {
                    if let Ok(text) = e.unescape() {
                        output.push_str(&text);
                    }
                }
            }
            Ok(Event::Eof) => break,
            Err(_) => break,
            _ => {}
        }
        buf.clear();
    }

    Ok(output)
}

fn local_name(name: &[u8]) -> &[u8] {
    // Strip namespace prefix (e.g. "w:t" → "t")
    name.splitn(2, |&b| b == b':').last().unwrap_or(name)
}

pub fn read_pdf(filepath: &str) -> Result<String, String> {
    contain_panics(|| pdf_extract::extract_text(filepath).map_err(|e| e.to_string()))
}

thread_local! {
    /// Set while this thread runs a parser inside `contain_panics`.
    static CONTAINING_PANIC: Cell<bool> = const { Cell::new(false) };
}

/// Runs a document parser, turning a panic into an `Err` so one malformed file
/// counts as unreadable instead of unwinding out of the whole search. pdf-extract
/// panics rather than erroring on content it doesn't model, such as /Indexed or
/// /DeviceN colour spaces.
///
/// Relies on unwinding: under `panic = "abort"` such a file still ends the process.
fn contain_panics<F>(parse: F) -> Result<String, String>
where
    F: FnOnce() -> Result<String, String> + UnwindSafe,
{
    // The default hook would print the parser's message and source location in the
    // middle of the search output, for a file that just gets counted as unreadable.
    // Under abort nothing is contained, so the message is left alone.
    static QUIET_HOOK: Once = Once::new();
    if cfg!(panic = "unwind") {
        QUIET_HOOK.call_once(|| {
            let previous_hook = panic::take_hook();
            panic::set_hook(Box::new(move |info| {
                if !CONTAINING_PANIC.with(Cell::get) {
                    previous_hook(info);
                }
            }));
        });
    }

    let outer = CONTAINING_PANIC.with(|c| c.replace(true));
    let result = panic::catch_unwind(parse);
    CONTAINING_PANIC.with(|c| c.set(outer));

    result.unwrap_or_else(|payload| {
        let message = payload
            .downcast_ref::<&str>()
            .copied()
            .or_else(|| payload.downcast_ref::<String>().map(String::as_str))
            .unwrap_or("unknown cause");
        Err(format!("parser panicked: {}", message))
    })
}
