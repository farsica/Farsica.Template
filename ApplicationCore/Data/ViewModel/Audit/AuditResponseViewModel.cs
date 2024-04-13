namespace Farsica.Template.Data.ViewModel.Audit
{
    public sealed class AuditResponseViewModel
    {
        public string? User { get; set; }

        public DateTimeOffset Date { get; set; }

        public string? UserAgent { get; set; }

        public string? IpAddress { get; set; }

        public IEnumerable<AuditEntryViewModel>? AuditEntries { get; set; }
    }
}
