using WebSpaceManager.DbAccess;
using WebSpaceManager.DTOs;
using WebSpaceManager.Entities;
using WebSpaceManager.Helpers.Result;
using WebSpaceManager.Models;

namespace WebSpaceManager.Helpers
{
    public class ContractsHelper : IContractsHelper, IContractsDevHelper
    {
        private readonly SpaceManagerDb _spaceManagerDb;

        public ContractsHelper(SpaceManagerDb spaceManagerDb)
        {
            _spaceManagerDb = spaceManagerDb ?? throw new NullReferenceException();
        }

        public List<GetContractDto> GetContracts()
        {
            var contracts = new List<GetContractDto>();

            contracts = _spaceManagerDb.Contracts.Join(_spaceManagerDb.Spaces,
                    contract => contract.SpaceId,
                    space => space.Id,
                    (contract, space) => new { Contract = contract, Space = space })
                .Join(_spaceManagerDb.Equipment,
                    cs => cs.Contract.EquipmentId,
                    equipment => equipment.Id,
                    (cs, equipment) => 
                        new GetContractDto() 
                        {
                            SpaceName = cs.Space.Name,
                            EquipmentName = equipment.Name,
                            EquipmentAmount = cs.Contract.Amount
                        }).ToList();

            return contracts;
        }

        public MyResult<GetContractDto> CreateContract(PostContractDto contractDto)
        {
            var space = _spaceManagerDb.Spaces.Find(contractDto.SpaceId);
            if (space == null)
                return MyResult<GetContractDto>.Failed($"Space not found");

            var equipment = _spaceManagerDb.Equipment.Find(contractDto.EquipmentId);
            if (equipment == null)
                return MyResult<GetContractDto>.Failed($"Equipment type not found");

            var allSpaceContracts = _spaceManagerDb.Contracts.Where(c => c.SpaceId.Equals(space.Id)).ToList();
            if(allSpaceContracts.Any())
            {
                var takenSquareMeters = Math.Ceiling(allSpaceContracts.Join(_spaceManagerDb.Equipment,
                    contract => contract.EquipmentId,
                    equipment => equipment.Id,
                    (contract, equipment) => new { Contract = contract, Equipment = equipment })
                    .Select(x => (x.Contract.Amount * x.Equipment.Space)).Sum());

                var spaceToTake= space.SquareMeters - takenSquareMeters - equipment.Space * contractDto.Amount;
                if (spaceToTake < 0)
                    return MyResult<GetContractDto>.Failed($"Not enough space!");
            }

            var contract = new Contract()
            {
                Id = new Guid(),
                SpaceId = space.Id,
                EquipmentId = equipment.Id,
                Amount = contractDto.Amount
            };

            _spaceManagerDb.Contracts.Add(contract);
            _spaceManagerDb.SaveChanges();

            var createdContractDto = new GetContractDto()
            {
                SpaceName = _spaceManagerDb.Spaces.Find(contract.SpaceId).Name,
                EquipmentName = _spaceManagerDb.Equipment.Find(contract.EquipmentId).Name,
                EquipmentAmount = contractDto.Amount
            };

            return MyResult<GetContractDto>.Successful(createdContractDto);
        }

        public List<Contract> GetContractsRaw()
        {
            return _spaceManagerDb.Contracts.ToList();
        }
    }
}
