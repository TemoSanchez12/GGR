using GGR.Server.Commands.Contracts;
using GGR.Shared;
using GGR.Shared.Reward;
using GGR.Server.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GGR.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RewardController : ControllerBase
{
  private readonly static string _genericErrorMessage = "Ocurrio un error inesperado";
  private readonly static string _genericSuccessMessage = "Todo ha salido correcto";
  private readonly static string _createRewardSuccessMessage = "Se ha creado la recompenza correctamente";
  private readonly static string _updateRewardSuccessMessage = "Se ha actualizado correctamente la recompenza";

  private IRewardCommands _rewardCommands;
  private ILogger<RewardController> _logger;

  public RewardController(IRewardCommands rewardCommands, ILogger<RewardController> logger)
  {
    _logger = logger;
    _rewardCommands = rewardCommands;
  }

  [HttpGet("get-all")]
  public async Task<ActionResult<ServiceResponse<GetAllRewardsReponse>>> GetAllRewards()
  {
    var response = new ServiceResponse<GetAllRewardsReponse>();

    try
    {
      var rewards = await _rewardCommands.GetAllRewards();
      response.Success = true;
      response.Message = _genericSuccessMessage;
      response.Data = new GetAllRewardsReponse { Rewards = rewards.Select(reward => reward.ToDefinition()).ToList() };
      return Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogError("Error while fetching all rewards {ErrorMessage}", ex.Message);
      response.Success = false;
      response.Message = _genericErrorMessage;
      return BadRequest(response);
    }
  }

  [HttpGet("get/{idReward}")]
  public async Task<ActionResult<ServiceResponse<GetRewardResponse>>> GetReward(string idReward)
  {
    var response = new ServiceResponse<GetRewardResponse>();
    try
    {
      var reward = await _rewardCommands.GetReward(Guid.Parse(idReward));
      response.Success = true;
      response.Message = _genericSuccessMessage;
      response.Data = new GetRewardResponse { Reward = reward.ToDefinition() };
      return Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogError("Error while fetching reward: {ErrorMessasge}", ex.Message);
      var error = (RewardError)Enum.Parse(typeof(RewardError), ex.Message);

      response.Success = false;
      response.Message = error switch
      {
        RewardError.SavingDataError => RewardErrorMessage.SavingDataError,
        RewardError.RewardNotFound => RewardErrorMessage.RewardNotFound,
        _ => _genericErrorMessage
      };
      return BadRequest(response);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPost("create-reward")]
  public async Task<ActionResult<CreateRewardResponse>> CreateReward(CreateRewardRequest request)
  {
    var response = new ServiceResponse<CreateRewardResponse>();

    try
    {
      var reward = await _rewardCommands.CreateReward(request);
      response.Success = true;
      response.Message = _createRewardSuccessMessage;
      response.Data = new CreateRewardResponse { Reward = reward.ToDefinition() };
      return Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogError("Error while creating reward: {ErrorMessasge}", ex.Message);
      var error = (RewardError)Enum.Parse(typeof(RewardError), ex.Message);

      response.Success = false;
      response.Message = error switch
      {
        RewardError.SavingDataError => RewardErrorMessage.SavingDataError,
        RewardError.RewardAlreadyExists => RewardErrorMessage.RewardAlreadyExists,
        _ => _genericErrorMessage
      };
      return BadRequest(response);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut("update-reward")]
  public async Task<ActionResult<ServiceResponse<UpdateRewardResponse>>> UpdateReward(UpdateRewardRequest request)
  {
    var response = new ServiceResponse<UpdateRewardResponse>();

    try
    {
      var reward = await _rewardCommands.UpdateReward(request);
      response.Success = true;
      response.Message = _updateRewardSuccessMessage;
      response.Data = new UpdateRewardResponse { reward = reward.ToDefinition() };
      return Ok(response);
    }
    catch (Exception ex)
    {
      _logger.LogError("Error while updating reward: {ErrorMessage}", ex.Message);

      var error = (RewardError)Enum.Parse(typeof(RewardError), ex.Message);

      response.Success = false;
      response.Message = error switch
      {
        RewardError.SavingDataError => RewardErrorMessage.SavingDataError,
        RewardError.RewardNotFound => RewardErrorMessage.RewardAlreadyExists,
        _ => _genericErrorMessage
      };
      return BadRequest(response);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("delete-reward")]
  public async Task<IActionResult> DeleteReward(DeleteRewardRequest request)
  {
    try
    {
      await _rewardCommands.DeleteReward(request);
      return Ok();
    }
    catch (Exception ex)
    {
      _logger.LogError("Error while updating reward: {ErrorMessage}", ex.Message);
      var error = (RewardError)Enum.Parse(typeof(RewardError), ex.Message);

      return error switch
      {
        RewardError.SavingDataError => BadRequest(RewardErrorMessage.SavingDataError),
        RewardError.RewardNotFound => BadRequest(RewardErrorMessage.RewardNotFound),
        _ => BadRequest()
      };
    }
  }

}
