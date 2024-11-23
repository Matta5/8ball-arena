using BLL.DTOs;

public class DuelDTO
{
    public int Id { get; set; }
    public string Status { get; set; }
    public DateTime DateCreated { get; set; }
    public int ParticipantUserId { get; set; }
    public List<DuelParticipantDTO> Participants { get; set; } = new List<DuelParticipantDTO>();
}
