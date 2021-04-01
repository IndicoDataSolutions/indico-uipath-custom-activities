using System;
using FluentAssertions.Specialized;
using Indico.Exception;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    public static class DelegateAssertionsExtensions
    {
        public static ExceptionAssertions<GraphQLException> ThrowAuthorizationException<TDelegate>(
            this DelegateAssertions<TDelegate> assertions)
            where TDelegate : Delegate =>
            assertions.Throw<GraphQLException>()
                .WithMessage(
                    @"*{""code"": 403, ""content"": {}, ""error_class"": ""ServiceException"", ""error_type"": ""ForbiddenAccess"", ""message"": ""This user does not have access to this resource""}");
    }
}
