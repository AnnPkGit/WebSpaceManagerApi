using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebSpaceManager.Controllers;
using WebSpaceManager.DTOs;
using WebSpaceManager.Helpers;
using WebSpaceManager.Helpers.Result;
using WebSpaceManager.Models;

namespace WebSpaceManagerTests
{
    [TestFixture]
    public class ContractsControllerTests
    {
        private Mock<IContractsHelper> _contractsHelper;
        private Mock<IContractsDevHelper> _contractsDevHelper;
        private Mock<IAuthoriztionValidator> _authoriztionValidator;
        private ContractsController _contractController;
        private const string _key = "key";

        [SetUp]
        public void Setup()
        {
            _contractsHelper = new Mock<IContractsHelper>();
            _contractsDevHelper = new Mock<IContractsDevHelper>();
            _authoriztionValidator = new Mock<IAuthoriztionValidator>();

            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Headers).Returns(new HeaderDictionary() { { "ApiSecret", _key } });
            context.Setup(x => x.Request).Returns(request.Object);
            _contractController = new ContractsController(_contractsHelper.Object, _authoriztionValidator.Object, _contractsDevHelper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = context.Object
                }
            };
        }

        [Test]
        public void Constructor_throws_NullReferenceException()
        {
            //Act, Assert
            Assert.Throws<NullReferenceException>(() => new ContractsController(
                null,
                _authoriztionValidator.Object,
                _contractsDevHelper.Object));
            Assert.Throws<NullReferenceException>(() => new ContractsController(
                _contractsHelper.Object,
                null,
                _contractsDevHelper.Object));
            Assert.Throws<NullReferenceException>(() => new ContractsController(
                _contractsHelper.Object,
                _authoriztionValidator.Object,
                null));
        }

        [Test]
        public void Constructor_passes()
        {
            //Act, Assert
            Assert.DoesNotThrow(() => new ContractsController(_contractsHelper.Object, _authoriztionValidator.Object, _contractsDevHelper.Object));
        }

        [Test]
        public void GetContracts_returns_GetContractDtosList()
        {
           //Arrange
            var contractsDtos = new List<GetContractDto>();
            _contractsHelper.Setup(x => x.GetContracts()).Returns(contractsDtos);
            _authoriztionValidator.Setup( x => x.IsValidKey(_key)).Returns(true);

            //Act
            var result = _contractController.GetContracts();

            //Assert
            Assert.AreEqual(contractsDtos, result.Value);
        }

        [Test]
        public void GetContracts_returns_Unauthorized()
        {
            //Arrange
            var expectedResult = new UnauthorizedResult();
            _authoriztionValidator.Setup(x => x.IsValidKey("WrongKey")).Returns(true);

            //Act
            var result = _contractController.GetContracts();

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, (result.Result as UnauthorizedResult).StatusCode);
        }

        [Test]
        public void PostContract_returns_Unauthorized()
        {
            //Arrange
            var expectedResult = new UnauthorizedResult();
            _authoriztionValidator.Setup(x => x.IsValidKey("WrongKey")).Returns(true);

            //Act
            var result = _contractController.PostContract(new PostContractDto());

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, (result as UnauthorizedResult).StatusCode);
        }


        [Test]
        public void PostContract_returns_GetContractDto()
        {
            //Arrange
            var postContract = new PostContractDto();
            var contractDto = new GetContractDto();
            var myResult = MyResult<GetContractDto>.Successful(contractDto);
            var expectedResult = new OkObjectResult(contractDto);
            _contractsHelper.Setup(x => x.CreateContract(postContract)).Returns(myResult);
            _authoriztionValidator.Setup(x => x.IsValidKey(_key)).Returns(true);

            //Act
            var result = _contractController.PostContract(postContract);

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, (result as OkObjectResult).StatusCode);
            Assert.AreEqual(expectedResult.Value, (result as OkObjectResult).Value);
        }

        [Test]
        public void PostContract_returns_BadRequest()
        {
            //Arrange
            var postContract = new PostContractDto();
            var myResult = MyResult<GetContractDto>.Failed("Not enough space!");
            var expectedResult = new BadRequestObjectResult(myResult.ErrorMessage);
            _contractsHelper.Setup(x => x.CreateContract(postContract)).Returns(myResult);
            _authoriztionValidator.Setup(x => x.IsValidKey(_key)).Returns(true);

            //Act
            var result = _contractController.PostContract(postContract);

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, (result as BadRequestObjectResult).StatusCode);
            Assert.AreEqual(expectedResult.Value, (result as BadRequestObjectResult).Value);
        }
    }
}