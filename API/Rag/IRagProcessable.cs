namespace KeepGrouped.API.Rag;

public interface IRagProcessable
{
    public string RagId { get; }
    public bool Ingested { get; set; }
}