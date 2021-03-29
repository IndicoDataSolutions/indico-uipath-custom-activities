using System.Activities;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using IndicoV2.Models.Models;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class GetModelGroupTests
    {
        private int _modelGroupId;

        [SetUp]
        public void SetUp() => _modelGroupId = new Helpers.TestDataHelper().GetModelGroupId();

        [Test]
        public void GetModelGroup_ShouldReturnModelGroup()
        {
            var modelGroup = new GetModelGroup() {ModelGroupID = _modelGroupId}.Invoke();

            modelGroup.Id.Should().BeGreaterThan(0);
            modelGroup.Name.Should().NotBeEmpty();
            modelGroup.Status.Should().NotBe(ModelStatus.CREATING);
            modelGroup.SelectedModel.Should().NotBeNull();
        }

        [Test]
        public void GetModelGroup_ShouldThrow_WhenNoModelGroupID() =>
            new GetModelGroup()
                .Invoking(action => action.Invoke())
                .Should().Throw<InvalidWorkflowException>()
                .WithMessage("*Value for a required activity argument 'ModelGroupID' was not supplied.");

        [Test]
        public void GetModelGroup_ShouldThrow_WhenInvalidModelGroupID() =>
            new GetModelGroup { ModelGroupID = 0 }
                .Invoking(action => action.Invoke())
                .Should()
                .ThrowAuthorizationException();
    }
}
