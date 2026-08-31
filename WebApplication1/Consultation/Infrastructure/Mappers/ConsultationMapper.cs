using WebApplication1.Consultation.Domain.Models.Entities;
using WebApplication1.Consultation.Domain.Models.Enum;

namespace WebApplication1.Consultation.Infrastructure.Mappers;

public static class ConsultationMapper
{
    public static Domain.Models.Aggregate.Consultation ToDomain(dynamic document)
    {
        var messages = ((IEnumerable<dynamic>)document.messages ?? Enumerable.Empty<dynamic>())
            .Select((dynamic message) =>
                new Message(
                    message.id,
                    message.senderId,
                    MessageSenderExtensions.FromString(message.senderRole),
                    message.content,
                    message.sentAt
                )
            ).ToList();

        return new Domain.Models.Aggregate.Consultation(
            document.id,
            document.patientId,
            document.motherId,
            document.nurseId,
            messages,
            document.createdAt,
            document.closedAt
        );
    }

    public static object ToPersistence(Domain.Models.Aggregate.Consultation consultation)
    {
        return consultation.ToPrimitives();
    }
}