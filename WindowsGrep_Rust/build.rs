fn main() {
    #[cfg(windows)]
    {
        let mut res = winresource::WindowsResource::new();
        res.set_icon("resources/grep.ico");
        res.compile().expect("failed to compile Windows resources");
    }
}
