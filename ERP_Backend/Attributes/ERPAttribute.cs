namespace ERP.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ERPAttribute(string AllowedFileTypes, int maxFileSize) : Attribute
    {
        public string AllowedFileTypes { get; set; } = AllowedFileTypes;
        public int MaxFileSize { get; set; } = maxFileSize;
    }
}