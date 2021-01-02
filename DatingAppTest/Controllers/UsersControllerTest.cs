using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using DatingApp.Controllers;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Helpers;
using DatingApp.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace DatingAppTest.Controllers
{
    public class UsersControllerTest
    {
        [Fact]
        public void GetUsers_whenValid_ReturnResult()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            
            var userId = 123;
            var userParams = fixture.Build<UserParams>()
                .With(propertyPicker: x => x.UserId, value: userId)
                .Create();

            var user = fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.Id, userId)
                .Create();

            var paginatedUsers = new PagedList<User>(
                new List<User>()
                {
                    new User()
                    {
                        Id = 12
                    }
                }
                , 1, 1, 1);
            var resultUsers = fixture.CreateMany<UserForListDto>();

            var test = paginatedUsers;

            var _datingRepo = fixture.Freeze<IDatingRepository>();
            _datingRepo.GetUser(userId).Returns(Task.FromResult(user));
            //only return if input parameter match 
            _datingRepo
                .GetUsersAsync(Arg.Is<UserParams>(x =>
                    x.UserId == userId ))
                .Returns(Task.FromResult(paginatedUsers));

            var _mapper = fixture.Freeze<IMapper>();
            _mapper.Map<IEnumerable<UserForListDto>>(paginatedUsers).Returns(resultUsers);


            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

            var sut = fixture.Build<UsersController>().OmitAutoProperties()
                .With(x => x.ControllerContext,
                    new ControllerContext()
                    {
                        HttpContext = new DefaultHttpContext()
                            {User = new ClaimsPrincipal(claimIdentity)}
                    })
                .Create();

            //Act
            var result = sut.GetUsers(userParams);

            //Assert

            (result.Result as OkObjectResult).Value.Should().Be(resultUsers);

            resultUsers = new List<UserForListDto>();
            (result.Result as OkObjectResult).Value.Should().NotBe(resultUsers);
        }
        
        [Theory]
        [InlineData("male","female")]
        [InlineData("female","male")]
        public void GetUsers_IfGenderNotProvided_RetrieveFromUserRepository(string gender, string genderQueried)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            var userId = 123;
            
            var userParams = fixture.Build<UserParams>()
                .With(propertyPicker: x => x.UserId, value: 1)
                .Without(x=>x.Gender).Create();

            var user = fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.Id, userId)
                .With(x=>x.Gender,gender)
                .Create();

            var paginatedUsers = new PagedList<User>(
                new List<User>()
                {
                    new User()
                    {
                        Id = 12
                    }
                }
                , 1, 1, 1);
            var resultUsers = fixture.CreateMany<UserForListDto>();

            var test = paginatedUsers;

            var _datingRepo = fixture.Freeze<IDatingRepository>();
            _datingRepo.GetUser(userId).Returns(Task.FromResult(user));
            //only return if input parameter match 
            _datingRepo
                .GetUsersAsync(Arg.Is<UserParams>(x =>
                    x.UserId == userId && x.Gender == genderQueried))
                .Returns(Task.FromResult(paginatedUsers));

            var _mapper = fixture.Freeze<IMapper>();
            _mapper.Map<IEnumerable<UserForListDto>>(paginatedUsers).Returns(resultUsers);


            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

            var sut = fixture.Build<UsersController>().OmitAutoProperties()
                .With(x => x.ControllerContext,
                    new ControllerContext()
                    {
                        HttpContext = new DefaultHttpContext()
                            {User = new ClaimsPrincipal(claimIdentity)}
                    })
                .Create();

            //Act
            var result = sut.GetUsers(userParams);

            //Assert

            (result.Result as OkObjectResult).Value.Should().Be(resultUsers);

            resultUsers = new List<UserForListDto>();
            (result.Result as OkObjectResult).Value.Should().NotBe(resultUsers);
        }
    }
}
