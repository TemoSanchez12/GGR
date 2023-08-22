using GGR.Server.Commands.Contracts;
using GGR.Server.Errors;
using GGR.Shared;
using GGR.Shared.RewardClaim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GGR.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RewardClaimController : ControllerBase
{
    private static string _genericErrorMessage = "Algo ha salido mal, intentelo de nuevo";
    private static string _successGetRewardClaimsMessage = "Se han devuelto las recompensas reclamadas";
    private static string _successCreateRewardClaimMessage = "Se ha creado correctamente el reclamo de recompensa";
    private static string _successUpdateRewardClaimStatusMessage = "Se ha actualizado correctamente el estado del reclamo de recompensa";

    private readonly ILogger<RewardClaimController> _logger;
    private readonly IRewardClaimCommands _rewardClaimCommands;

    public RewardClaimController(ILogger<RewardClaimController> logger, IRewardClaimCommands rewardClaimCommands)
    {
        _logger = logger;
        _rewardClaimCommands = rewardClaimCommands;
    }

    [HttpGet("get-all-reward-claims")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<GetAllRewardClaimsResponse>>> GetAllRewardClaims()
    {
        var response = new ServiceResponse<GetAllRewardClaimsResponse>();

        try
        {
            var rewardClaims = await _rewardClaimCommands.GetAllRewardClaims();
            response.Success = true;
            response.Message = _successGetRewardClaimsMessage;
            response.Data = new GetAllRewardClaimsResponse { RewardClaims = rewardClaims.Select(r => r.ToDefinition()).ToList() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while creating reward claim: {ErrorMessage}", ex.Message);
            response.Success = false;
            response.Message = _genericErrorMessage;
            return BadRequest(response);
        }
    }

    [HttpGet("get-reward-claims-email/{email}")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<GetAllRewardClaimsResponse>>> GetRewardClaimsByEmail(string email)
    {
        var response = new ServiceResponse<GetAllRewardClaimsResponse>();

        try
        {
            var rewardClaims = await _rewardClaimCommands.GetRewardClaimsByUserEmail(email);
            response.Success = true;
            response.Message = _successGetRewardClaimsMessage;
            response.Data = new GetAllRewardClaimsResponse { RewardClaims = rewardClaims.Select(r => r.ToDefinition()).ToList() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while creating reward claim: {ErrorMessage}", ex.Message);
            response.Success = false;
            response.Message = _genericErrorMessage;
            return BadRequest(response);
        }
    }

    [HttpPost("create-reward-claim")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<CreateRewardClaimResponse>>> CreateRewardClaim(CreateRewardClaimRequest request)
    {
        var response = new ServiceResponse<CreateRewardClaimResponse>();

        try
        {
            var rewardClaim = await _rewardClaimCommands.CreateRewardClaim(request);
            response.Success = true;
            response.Message = _successCreateRewardClaimMessage;
            response.Data = new CreateRewardClaimResponse { RewardClaim = rewardClaim.ToDefinition() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while creating reward claim: {ErrorMessage}", ex.Message);
            var error = (RewardClaimError) Enum.Parse(typeof(RewardClaimError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                RewardClaimError.UserNotFound => RewardClaimErrorMessage.UserNotFound,
                RewardClaimError.UserNotVerified => RewardClaimErrorMessage.UserNotVerified,
                RewardClaimError.RewardNotFound => RewardClaimErrorMessage.RewardNotFound,
                RewardClaimError.NotEnoughPoints => RewardClaimErrorMessage.NotEnoughPoints,
                RewardClaimError.ErrorSavingData => RewardClaimErrorMessage.ErrorSavingData,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
        }
    }

    [HttpPut("update-reward-claim-status")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<UpdateRewardClaimStatusResponse>>> UpdateRewardClaimStatus(UpdateRewardClaimStatusRequest request)
    {
        var response = new ServiceResponse<UpdateRewardClaimStatusResponse>();
        try
        {
            var rewardClaim = await _rewardClaimCommands.UpdateRewardClaimStatus(request);
            response.Success = true;
            response.Message = _successUpdateRewardClaimStatusMessage;
            response.Data = new UpdateRewardClaimStatusResponse { RewardClaim = rewardClaim.ToDefinition() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while updating reward claim status: {ErrorMessage}", ex.Message);
            var error = (RewardClaimError) Enum.Parse(typeof(RewardClaimError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                RewardClaimError.RewardClaimNotFound => RewardClaimErrorMessage.RewardClaimNotFound,
                RewardClaimError.ErrorSavingData => RewardClaimErrorMessage.ErrorSavingData,
                RewardClaimError.NoAllowToAssignStatus => RewardClaimErrorMessage.NoAllowToAssignStatus,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
        }
    }

    [HttpPut("update-reward-claim-status-admin")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<UpdateRewardClaimStatusResponse>>> UpdateRewardClaimStatusAdmin(UpdateRewardClaimStatusRequest request)
    {
        var response = new ServiceResponse<UpdateRewardClaimStatusResponse>();
        try
        {
            var rewardClaim = await _rewardClaimCommands.UpdateRewardClaimStatus(request);
            response.Success = true;
            response.Message = _successUpdateRewardClaimStatusMessage;
            response.Data = new UpdateRewardClaimStatusResponse { RewardClaim = rewardClaim.ToDefinition() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while updating reward claim status: {ErrorMessage}", ex.Message);
            var error = (RewardClaimError) Enum.Parse(typeof(RewardClaimError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                RewardClaimError.RewardClaimNotFound => RewardClaimErrorMessage.RewardClaimNotFound,
                RewardClaimError.ErrorSavingData => RewardClaimErrorMessage.ErrorSavingData,
                RewardClaimError.NoAllowToAssignStatus => RewardClaimErrorMessage.NoAllowToAssignStatus,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
        }
    }
}
