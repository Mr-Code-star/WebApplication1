using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Contexts.PatientManagement.Domain.Commands;
using WebApplication1.patient_management.Domain;
using WebApplication1.patient_management.Domain.ValueObjects;
using WebApplication1.patient_management.Interfaces.Assembler;
using WebApplication1.shared.Attributes;

namespace WebApplication1.patient_management.Interfaces;

using Microsoft.AspNetCore.Mvc;
using WebApplication1.patient_management.Domain.Commands;
using WebApplication1.patient_management.Domain.Queries;
using WebApplication1.patient_management.Interfaces.Facades;


[ApiController]
[Route("api/patients")]
[Authorize]
public class PatientManagementController : ControllerBase
{
    private readonly PatientManagementFacade _patientFacade;

    public PatientManagementController(PatientManagementFacade patientFacade)
    {
        _patientFacade = patientFacade;
    }

    // ==========================================
    // 1. REGISTRAR PACIENTE
    // ==========================================

    [HttpPost("register")]
    [Authorize]
    [RequireRole("Mother")] 
    public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientCommand command)
    {
        try
        {
            // Obtener motherId del token (desde HttpContext.User)
            var motherId = User.FindFirst("motherId")?.Value;

            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID no encontrado en el token" });
            }

            // Sobrescribir motherId con la del token
            var updatedCommand = new RegisterPatientCommand(
                command.Name,
                command.LastName,
                command.BirthDate,
                command.Gender,
                command.Weight,
                command.Height,
                motherId
            );

            await _patientFacade.RegisterPatientAsync(updatedCommand);

            return StatusCode(201, new { message = "Patient registered successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 2. ASIGNAR PACIENTE A ENFERMERA
    // ==========================================

    [HttpPost("assign-nurse")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> AssignPatientToNurse([FromBody] AssignPatientRequest request)
    {
        try
        {
            // ✅ LOG de todos los claims
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}");
            Console.WriteLine($"🔍 Claims: {string.Join(", ", claims)}");

            // ✅ Obtener nurseId del token
            var nurseId = User.FindFirst("nurseId")?.Value;
            
            // Si no existe, intentar con "id"
            if (string.IsNullOrEmpty(nurseId))
            {
                nurseId = User.FindFirst("id")?.Value;
                Console.WriteLine($"🔍 nurseId obtenido de 'id': {nurseId}");
            }
            // Si no existe, intentar con ClaimTypes.NameIdentifier
            if (string.IsNullOrEmpty(nurseId))
            {
                nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"🔍 nurseId obtenido de NameIdentifier: {nurseId}");
            }

            Console.WriteLine($"🔍 nurseId final: {nurseId ?? "NULL"}");
            Console.WriteLine($"🔍 patientId: {request.PatientId}");

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(request.PatientId))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            // ✅ Verificar que la enfermera existe
            // (asumiendo que tienes acceso a IUserRepository)
            // var nurse = await _userRepository.FindNurseByIdAsync(nurseId);
            // if (nurse == null)
            // {
            //     return BadRequest(new { error = "Nurse no encontrada" });
            // }

            var command = new AssignPatientToNurseCommand(request.PatientId, nurseId);

            await _patientFacade.AssignPatientToNurseAsync(command);

            return StatusCode(201, new { message = "Patient assigned successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en AssignPatientToNurse: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ==========================================
    // 3. CREAR HISTORIA CLÍNICA
    // ==========================================

    [HttpPost("medical-record")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordRequest request)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(request.PatientId) || request.Weight == null ||
                request.Height == null || string.IsNullOrEmpty(request.MotivoConsulta) ||
                string.IsNullOrEmpty(request.Observaciones))
            {
                return BadRequest(new
                {
                    error = "Faltan campos requeridos: patientId, weight, height, motivoConsulta, observaciones"
                });
            }

            // Validar que el paciente esté asignado a esta enfermera
            await _patientFacade.ValidateNurseHasPatientAsync(nurseId, request.PatientId);

            var command = new CreateInitialMedicalRecordCommand(
                request.PatientId,
                request.Weight.Value,
                request.Height.Value,
                request.MotivoConsulta,
                request.Observaciones,
                request.Antecedentes,
                request.Sintomas
            );

            await _patientFacade.CreateInitialMedicalRecordAsync(command);

            return StatusCode(201, new { message = "Medical record created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 4. REGISTRAR CONTROL DE HEMOGLOBINA
    // ==========================================

    [HttpPost("hemoglobin-control")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> RegisterHemoglobinControl([FromBody] RegisterHemoglobinRequest request)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(request.PatientId) || request.HemoglobinLevel == null)
            {
                return BadRequest(new { error = "Faltan campos: patientId, hemoglobinLevel" });
            }

            await _patientFacade.ValidateNurseHasPatientAsync(nurseId, request.PatientId);

            var command = new RegisterHemoglobinControlCommand(request.PatientId, request.HemoglobinLevel.Value);

            await _patientFacade.RegisterHemoglobinControlAsync(command);

            return StatusCode(201, new { message = "Hemoglobin control registered successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 5. ACTUALIZAR HISTORIA CLÍNICA
    // ==========================================

    [HttpPut("medical-record/update")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> UpdateMedicalRecord([FromBody] UpdateMedicalRecordRequest request)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(request.PatientId))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            await _patientFacade.ValidateNurseHasPatientAsync(nurseId, request.PatientId);

            var command = new UpdateMedicalRecordCommand(
                request.PatientId,
                request.Weight,
                request.Height,
                request.MotivoConsulta,
                request.Observaciones,
                request.Antecedentes,
                request.Sintomas
            );

            await _patientFacade.UpdateMedicalRecordAsync(command);

            return Ok(new { message = "Medical record updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 6. DAR DE ALTA PACIENTE
    // ==========================================

    [HttpPut("discharge")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> DischargePatient([FromBody] DischargePatientRequest request)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(request.PatientId))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            await _patientFacade.ValidateNurseHasPatientAsync(nurseId, request.PatientId);

            var command = new DischargePatientCommand(request.PatientId, nurseId);

            await _patientFacade.DischargePatientAsync(command);

            return Ok(new { message = "Patient discharged successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 7. LISTAR PACIENTES POR MADRE
    // ==========================================

    [HttpGet("mother/{motherId}")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> ListPatientsByMother(string motherId)
    {
        try
        {
            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID es requerido" });
            }

            var query = new ListPatientsByMotherQuery(motherId);
            var patients = await _patientFacade.ListPatientsByMotherAsync(query);

            return Ok(patients);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 8. OBTENER HISTORIA CLÍNICA
    // ==========================================

    [HttpGet("{patientId}/medical-record")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> GetMedicalRecord(string patientId)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            await _patientFacade.ValidateNurseHasPatientAsync(nurseId, patientId);

            var query = new GetMedicalRecordQuery(patientId);
            var data = await _patientFacade.GetMedicalRecordAsync(query);

            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 9. OBTENER HISTORIAL DE HEMOGLOBINA
    // ==========================================

    [HttpGet("medical-record/{medicalRecordId}/controls")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> GetHemoglobinHistory(string medicalRecordId)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(medicalRecordId))
            {
                return BadRequest(new { error = "Medical Record ID es requerido" });
            }

            await _patientFacade.ValidateNurseHasAccessToMedicalRecordAsync(nurseId, medicalRecordId);

            var query = new GetHemoglobinControlsHistoryQuery(medicalRecordId);
            var history = await _patientFacade.GetHemoglobinControlsHistoryAsync(query);

            var resource = HemoglobinHistoryResourceAssembler.ToResource(history);

            return Ok(resource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 10. PACIENTES ELEGIBLES PARA ALTA
    // ==========================================

    [HttpGet("discharge/nurse")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> GetEligiblePatientsForDischarge([FromQuery] string? searchTerm = null)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            var query = new GetPatientsEligibleForDischargeQuery(nurseId, searchTerm);
            var patients = await _patientFacade.GetPatientsEligibleForDischargeAsync(query);

            var result = new
            {
                success = true,
                status = patients.Count == 0 ? "EMPTY" : "SUCCESS",
                message = patients.Count == 0 ? "No hay pacientes elegibles para alta en este momento" : "Pacientes elegibles para alta recuperados exitosamente",
                data = new
                {
                    patients,
                    total = patients.Count,
                    searchTerm,
                    nurseId
                }
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, status = "ERROR", error = ex.Message });
        }
    }

    // ==========================================
    // 11. DESCARGAR PDF HISTORIA CLÍNICA
    // ==========================================

    [HttpGet("medical-record/{medicalRecordId}/pdf")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> DownloadMedicalRecordPdf(string medicalRecordId)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(medicalRecordId))
            {
                return BadRequest(new { error = "Medical Record ID es requerido" });
            }

            await _patientFacade.ValidateNurseHasAccessToMedicalRecordAsync(nurseId, medicalRecordId);

            var query = new DownloadMedicalRecordPdfQuery(medicalRecordId);
            var pdf = await _patientFacade.DownloadMedicalRecordPdfAsync(query);

            return File(pdf, "application/pdf", "medical-record.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 12. DESCARGAR REPORTE HEMOGLOBINA PDF
    // ==========================================

    [HttpGet("medical-record/{medicalRecordId}/hemoglobin-report")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> DownloadHemoglobinReportPdf(string medicalRecordId)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(medicalRecordId))
            {
                return BadRequest(new { error = "Medical Record ID es requerido" });
            }

            await _patientFacade.ValidateNurseHasAccessToMedicalRecordAsync(nurseId, medicalRecordId);

            var query = new DownloadHemoglobinReportPdfQuery(medicalRecordId);
            var pdf = await _patientFacade.DownloadHemoglobinReportPdfAsync(query);

            return File(pdf, "application/pdf", "hemoglobin-report.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 13. BUSCAR MADRE POR DNI
    // ==========================================

    [HttpGet("mother/search/{searchTerm}")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> SearchMotherByDni(string searchTerm)
    {
        try
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(new { error = "Search term es requerido" });
            }

            var query = new SearchMotherByDniQuery(searchTerm);
            var mothers = await _patientFacade.SearchMotherByDniAsync(query);

            return Ok(mothers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 14. PACIENTES ASIGNADOS A ENFERMERA
    // ==========================================

    [HttpGet("nurse")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> GetPatientsAssignedToNurse([FromQuery] string? searchTerm = null)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            var query = new GetPatientsAssignedToNurseQuery(nurseId, searchTerm);
            var patients = await _patientFacade.GetPatientsAssignedToNurseAsync(query);

            var result = new
            {
                success = true,
                status = patients.Count == 0 ? "EMPTY" : "SUCCESS",
                message = patients.Count == 0 ? "No tienes pacientes asignados actualmente" : "Pacientes asignados recuperados exitosamente",
                data = new
                {
                    patients,
                    total = patients.Count,
                    searchTerm,
                    nurseId
                }
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, status = "ERROR", error = ex.Message });
        }
    }

    // ==========================================
    // 15. EVOLUCIÓN DE HEMOGLOBINA (GRÁFICO)
    // ==========================================

    [HttpGet("{patientId}/hemoglobin-evolution")]
    [Authorize]
    [RequireRole("Mother")]
    public async Task<IActionResult> GetHemoglobinEvolutionChart(string patientId)
    {
        try
        {
            var motherId = User.FindFirst("motherId")?.Value;

            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID no encontrado" });
            }

            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            await _patientFacade.ValidatePatientBelongsToMotherAsync(patientId, motherId);

            var query = new GetHemoglobinEvolutionChartQuery(patientId);
            var result = await _patientFacade.GetHemoglobinEvolutionChartAsync(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 16. CONTEO DE PACIENTES ACTIVOS
    // ==========================================

    [HttpGet("nurse/active-count")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> GetActivePatientsCount()
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            var query = new GetActivePatientsCountQuery(nurseId);
            var count = await _patientFacade.GetActivePatientsCountAsync(query);

            return Ok(new { nurseId, activePatientsCount = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 17. MIS PACIENTES (MADRE)
    // ==========================================

    [HttpGet("my-patients")]
    [Authorize]
    [RequireRole("Mother")] 
    public async Task<IActionResult> GetMyPatients()
    {
        try
        {
            var motherId = User.FindFirst("motherId")?.Value;

            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID no encontrado en el token" });
            }

            var query = new GetMotherPatientsSummaryQuery(motherId);
            var patients = await _patientFacade.GetMotherPatientsSummaryAsync(query);

            return Ok(new { motherId, patients });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 18. INFORMACIÓN BÁSICA DE PACIENTE
    // ==========================================

    [HttpGet("{id}")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> GetPatientBasicInfo(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            var query = new GetPatientBasicInfoQuery(id);
            var patient = await _patientFacade.GetPatientBasicInfoAsync(query);

            if (patient == null)
            {
                return NotFound(new { error = "Patient not found" });
            }

            return Ok(patient);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 19. VERIFICAR SI TIENE HISTORIA CLÍNICA
    // ==========================================

    [HttpGet("{patientId}/medical-record/check")]
    [Authorize]
    [RequireRole("Nurse")] 
    public async Task<IActionResult> CheckPatientMedicalRecord(string patientId)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            await _patientFacade.ValidateNurseHasPatientAsync(nurseId, patientId);

            var result = await _patientFacade.CheckPatientMedicalRecordAsync(patientId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

// ==========================================
// REQUEST DTOs
// ==========================================

public class AssignPatientRequest
{
    public string PatientId { get; set; } = string.Empty;
}

public class CreateMedicalRecordRequest
{
    public string PatientId { get; set; } = string.Empty;
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public string MotivoConsulta { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public List<Antecedente>? Antecedentes { get; set; }
    public List<string>? Sintomas { get; set; }
}

public class RegisterHemoglobinRequest
{
    public string PatientId { get; set; } = string.Empty;
    public double? HemoglobinLevel { get; set; }
}

public class UpdateMedicalRecordRequest
{
    public string PatientId { get; set; } = string.Empty;
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public string? MotivoConsulta { get; set; }
    public string? Observaciones { get; set; }
    public List<Antecedente>? Antecedentes { get; set; }
    public List<string>? Sintomas { get; set; }
}

public class DischargePatientRequest
{
    public string PatientId { get; set; } = string.Empty;
}