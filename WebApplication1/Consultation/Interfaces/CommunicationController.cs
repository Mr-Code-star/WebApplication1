using WebApplication1.Consultation.Interfaces.Assemblers;
using WebApplication1.Consultation.Interfaces.Facades;
using WebApplication1.Consultation.Interfaces.Resources;
using WebApplication1.shared.Attributes;

namespace WebApplication1.Consultation.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/communication")]
[Authorize] 
public class CommunicationController : ControllerBase
{
    private readonly CommunicationFacade _facade;

    public CommunicationController(CommunicationFacade facade)
    {
        _facade = facade;
    }

    // ==========================================
    // 1. INICIAR CONSULTA - SOLO MADRE
    // ==========================================

    [HttpPost("consultations")]
    [RequireRole("Mother")] // Solo MOTHER
    public async Task<IActionResult> StartConsultation([FromBody] StartConsultationRequest request)
    {
        try
        {
            var motherId = User.FindFirst("motherId")?.Value;

            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(request.PatientId) || string.IsNullOrEmpty(request.FirstMessageContent))
            {
                return BadRequest(new { error = "Faltan campos requeridos: patientId, firstMessageContent" });
            }

            var resource = new StartConsultationResource
            {
                MotherId = motherId,
                PatientId = request.PatientId,
                FirstMessageContent = request.FirstMessageContent
            };

            var command = StartConsultationCommandFromResourceAssembler.ToCommand(resource);
            var result = await _facade.StartConsultationAsync(command);

            return StatusCode(201, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 2. ENVIAR MENSAJE - MADRE O ENFERMERA
    // ==========================================

    [HttpPost("messages")]
    [RequireRole("Mother", "Nurse")] // MOTHER o NURSE
    public async Task<IActionResult> AddMessage([FromBody] AddMessageRequest request)
    {
        try
        {
            var senderId = User.FindFirst("id")?.Value;
            var senderRole = User.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(senderRole))
            {
                return BadRequest(new { error = "Usuario no autenticado correctamente" });
            }

            if (string.IsNullOrEmpty(request.ConsultationId) || string.IsNullOrEmpty(request.Content))
            {
                return BadRequest(new { error = "Faltan campos requeridos: consultationId, content" });
            }

            var resource = new AddMessageResource
            {
                ConsultationId = request.ConsultationId,
                SenderId = senderId,
                SenderRole = senderRole.ToUpper(),
                Content = request.Content
            };

            var command = AddMessageCommandFromResourceAssembler.ToCommand(resource);
            var result = await _facade.AddMessageAsync(command);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 3. CERRAR CONSULTA - SOLO ENFERMERA
    // ==========================================

    [HttpDelete("consultations/close")]
    [RequireRole("Nurse")] // Solo NURSE
    public async Task<IActionResult> CloseConsultation([FromBody] CloseConsultationRequest request)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(request.ConsultationId))
            {
                return BadRequest(new { error = "Falta campo requerido: consultationId" });
            }

            var resource = new CloseConsultationResource
            {
                ConsultationId = request.ConsultationId,
                NurseId = nurseId
            };

            var command = CloseConsultationCommandFromResourceAssembler.ToCommand(resource);
            var result = await _facade.CloseConsultationAsync(command);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 4. OBTENER PACIENTES CON ENFERMERA ASIGNADA - SOLO MADRE
    // ==========================================
    [HttpGet("patients")]
    [RequireRole("Mother")] // Solo MOTHER
    public async Task<IActionResult> GetPatientsWithNurseAssignment()
    {
        try
        {
            var motherId = User.FindFirst("motherId")?.Value;

            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID no encontrado en el token" });
            }

            var query = GetPatientsWithNurseAssignmentQueryAssembler.ToQuery(motherId);
            var result = await _facade.GetPatientsWithNurseAssignmentAsync(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 5. OBTENER INFORMACIÓN DE ENFERMERA - SOLO MADRE
    // ==========================================
    [HttpGet("nurse-info/{patientId}")]
    [RequireRole("Mother")] // Solo MOTHER
    public async Task<IActionResult> GetNurseInfoForConsultation(string patientId)
    {
        try
        {
            var motherId = User.FindFirst("motherId")?.Value;

            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID no encontrado en el token" });
            }

            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest(new { error = "Patient ID es requerido" });
            }

            var query = GetNurseInfoForConsultationQueryAssembler.ToQuery(patientId);
            var result = await _facade.GetNurseInfoForConsultationAsync(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 6. OBTENER CHAT DE CONSULTA - MADRE O ENFERMERA
    // ==========================================
    
    [HttpGet("chat/{consultationId}")]
    [RequireRole("Mother", "Nurse")] // MOTHER o NURSE
    public async Task<IActionResult> GetConsultationChat(string consultationId)
    {
        try
        {
            var requesterId = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(requesterId))
            {
                return BadRequest(new { error = "Usuario no autenticado correctamente" });
            }

            if (string.IsNullOrEmpty(consultationId))
            {
                return BadRequest(new { error = "Consultation ID es requerido" });
            }

            var query = GetConsultationChatQueryAssembler.ToQuery(consultationId, requesterId);
            var result = await _facade.GetConsultationChatAsync(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 7. OBTENER CONSULTAS DE MADRE - SOLO MADRE
    // ==========================================
    [HttpGet("consultations/mother")]
    [RequireRole("Mother")] // Solo MOTHER
    public async Task<IActionResult> GetOpenConsultationsByMother()
    {
        try
        {
            var motherId = User.FindFirst("motherId")?.Value;

            if (string.IsNullOrEmpty(motherId))
            {
                return BadRequest(new { error = "Mother ID no encontrado en el token" });
            }

            var query = GetOpenConsultationsByMotherQueryAssembler.ToQuery(motherId);
            var result = await _facade.GetOpenConsultationsByMotherAsync(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 8. OBTENER CONSULTAS DE ENFERMERA - SOLO ENFERMERA
    // ==========================================
    [HttpGet("consultations/nurse")]
    [RequireRole("Nurse")] // Solo NURSE
    public async Task<IActionResult> GetOpenConsultationsByNurse([FromQuery] string? searchTerm = null)
    {
        try
        {
            var nurseId = User.FindFirst("nurseId")?.Value;

            if (string.IsNullOrEmpty(nurseId))
            {
                return BadRequest(new { error = "Nurse ID no encontrado en el token" });
            }

            var query = GetOpenConsultationsByNurseQueryAssembler.ToQuery(nurseId, searchTerm);
            var result = await _facade.GetOpenConsultationsByNurseAsync(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==========================================
    // 9. OBTENER MENSAJES DESPUÉS DE TIMESTAMP - MADRE O ENFERMERA
    // ==========================================
    [HttpGet("chat/{consultationId}/messages/after")]
    [RequireRole("Mother", "Nurse")] // MOTHER o NURSE
    public async Task<IActionResult> GetMessagesAfter(
        string consultationId,
        [FromQuery] long afterTimestamp,
        [FromQuery] int? limit = null)
    {
        try
        {
            var requesterId = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(requesterId))
            {
                return BadRequest(new { error = "Usuario no autenticado correctamente" });
            }

            if (string.IsNullOrEmpty(consultationId))
            {
                return BadRequest(new { error = "Consultation ID es requerido" });
            }

            if (afterTimestamp <= 0)
            {
                return BadRequest(new { error = "afterTimestamp es requerido y debe ser un número mayor a 0" });
            }

            var query = GetMessagesAfterQueryAssembler.ToQuery(consultationId, requesterId, afterTimestamp, limit);
            var result = await _facade.GetMessagesAfterAsync(query);

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

public class StartConsultationRequest
{
    public string PatientId { get; set; } = string.Empty;
    public string FirstMessageContent { get; set; } = string.Empty;
}

public class AddMessageRequest
{
    public string ConsultationId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class CloseConsultationRequest
{
    public string ConsultationId { get; set; } = string.Empty;
}