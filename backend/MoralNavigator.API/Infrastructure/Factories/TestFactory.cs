using MoralNavigator.API.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MoralNavigator.API.Infrastructure.Factories
{
    public static class TestFactory
    {
        public static Test Create(string title, IEnumerable<Question> questions) => new Test
        {
            Title = title,
            Questions = questions.ToList()
        };
    }
}