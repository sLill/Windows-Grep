use std::io::Read;

/// Reads text from a DOCX file by unzipping it and extracting `<w:t>` text nodes.
pub fn read_docx(filepath: &str) -> Result<String, String> {
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

/// PDF text extraction is not supported in this Rust port.
pub fn read_pdf(_filepath: &str) -> Result<String, String> {
    Err("PDF text extraction is not supported in this Rust port".to_string())
}
