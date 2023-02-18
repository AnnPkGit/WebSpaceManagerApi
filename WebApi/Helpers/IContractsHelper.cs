using WebSpaceManager.DTOs;
using WebSpaceManager.Helpers.Result;
using WebSpaceManager.Models;

namespace WebSpaceManager.Helpers
{
    public interface IContractsHelper
    {
        List<GetContractDto> GetContracts();

        MyResult<GetContractDto> CreateContract(PostContractDto contractDto);
    }
}
