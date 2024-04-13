namespace Farsica.Template.Data.ViewModel.Audit
{
    using NUlid;

    public sealed class AuditListResponseViewModel
    {
        public Ulid Id { get; set; }

        public string? User { get; set; }

        public DateTimeOffset Date { get; set; }

        public string? IpAddress { get; set; }
    }
}
