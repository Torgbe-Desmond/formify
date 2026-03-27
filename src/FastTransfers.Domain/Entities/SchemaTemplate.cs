using FastTransfers.Domain.Common;

namespace FastTransfers.Domain.Entities;

/// <summary>
/// One schema template per folder. Stores the YAML field definitions,
/// HTML layout, and CSS styles separately. Always persisted in SQL Server
/// since it is small, text-based, and frequently edited.
/// </summary>
public class SchemaTemplate : BaseEntity
{
    public Guid FolderId { get; private set; }

    /// <summary>YAML that defines the form fields (label, type, required, etc.)</summary>
    public string SchemaYaml { get; private set; } = string.Empty;

    /// <summary>Liquid-powered HTML template for rendering documents.</summary>
    public string TemplateHtml { get; private set; } = string.Empty;

    /// <summary>CSS styles scoped to the rendered document.</summary>
    public string TemplateCss { get; private set; } = string.Empty;

    // Navigation
    public Folder Folder { get; private set; } = null!;

    private SchemaTemplate() { } // EF

    public static SchemaTemplate Create(Guid folderId,
                                        string schemaYaml    = "",
                                        string templateHtml  = "",
                                        string templateCss   = "")
    {
        return new SchemaTemplate
        {
            FolderId     = folderId,
            SchemaYaml   = schemaYaml,
            TemplateHtml = templateHtml,
            TemplateCss  = templateCss
        };
    }

    public void Update(string schemaYaml, string templateHtml, string templateCss)
    {
        SchemaYaml   = schemaYaml;
        TemplateHtml = templateHtml;
        TemplateCss  = templateCss;
        SetUpdated();
    }
}
