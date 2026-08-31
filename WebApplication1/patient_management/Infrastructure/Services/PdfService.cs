namespace WebApplication1.patient_management.Infrastructure.Services;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;


public static class PdfService
{
    public static async Task<byte[]> GenerateMedicalRecordPdfAsync(object patient, object medicalRecord)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);

        document.Add(new Paragraph("Medical Record Report").SetFontSize(18));
        document.Add(new Paragraph($"Patient: {GetProperty(patient, "Name")} {GetProperty(patient, "LastName")}"));
        document.Add(new Paragraph($"Gender: {GetProperty(patient, "Gender")}"));
        document.Add(new Paragraph($"Status: {GetProperty(patient, "Status")}"));
        document.Add(new Paragraph($"Weight: {GetProperty(medicalRecord, "Weight")} kg"));
        document.Add(new Paragraph($"Height: {GetProperty(medicalRecord, "Height")} cm"));
        document.Add(new Paragraph($"Hemoglobin Level: {GetProperty(medicalRecord, "HemoglobinLevel") ?? "Not registered"}"));
        document.Add(new Paragraph($"Consult Reason: {GetProperty(medicalRecord, "MotivoConsulta")}"));
        document.Add(new Paragraph($"Observations: {GetProperty(medicalRecord, "Observaciones") ?? "None"}"));

        document.Add(new Paragraph("Symptoms:"));
        var sintomas = GetProperty(medicalRecord, "Sintomas") as List<string> ?? new List<string>();
        foreach (var symptom in sintomas)
        {
            document.Add(new Paragraph($"- {symptom}"));
        }

        document.Add(new Paragraph("Antecedents:"));
        var antecedentes = GetProperty(medicalRecord, "Antecedentes") as List<dynamic> ?? new List<dynamic>();
        foreach (var a in antecedentes)
        {
            document.Add(new Paragraph($"- {GetProperty(a, "Type")}: {GetProperty(a, "Description")}"));
        }

        document.Close();
        return memoryStream.ToArray();
    }

    public static async Task<byte[]> GenerateHemoglobinReportPdfAsync(object medicalRecord)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);

        document.Add(new Paragraph("Hemoglobin Controls Report").SetFontSize(18));

        var controls = GetProperty(medicalRecord, "Controls") as List<dynamic> ?? new List<dynamic>();

        if (controls.Count == 0)
        {
            document.Add(new Paragraph("No hemoglobin controls registered yet."));
            document.Close();
            return memoryStream.ToArray();
        }

        double total = 0;
        foreach (var control in controls)
        {
            var hemoglobin = (double?)GetProperty(control, "HemoglobinLevel");
            if (hemoglobin.HasValue)
            {
                total += hemoglobin.Value;
                document.Add(new Paragraph($"Date: {GetProperty(control, "Date")}"));
                document.Add(new Paragraph($"Hemoglobin: {hemoglobin.Value} g/dL"));
                document.Add(new Paragraph($"Status: {GetProperty(control, "AnemiaStatus") ?? "N/A"}"));
            }
        }

        var average = controls.Count > 0 ? total / controls.Count : 0;

        document.Add(new Paragraph($"Total Controls: {controls.Count}"));
        document.Add(new Paragraph($"Average Hemoglobin: {average:F2} g/dL"));

        document.Close();
        return memoryStream.ToArray();
    }

    private static object? GetProperty(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
}