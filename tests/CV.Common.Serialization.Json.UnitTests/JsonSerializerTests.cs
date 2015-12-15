using System;
using FluentAssertions;
using Jil;
using NUnit.Framework;

namespace CV.Common.Serialization.Json.UnitTests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        public class ToSerialize
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime? Date { get; set; }
        }

        private static readonly Options Options = Options.ISO8601IncludeInherited;

        private static readonly object[][] SerializationExamples = {
            new object[]
            {
                new ToSerialize { Id = 1, Name = "name1", Date = DateTime.Today },
                JSON.Serialize(new ToSerialize { Id = 1, Name = "name1", Date = DateTime.Today }, Options)
            },
            new object[]
            {
                new ToSerialize { Id = 2, Name = "name2", Date = null },
                JSON.Serialize(new ToSerialize { Id = 2, Name = "name2", Date = null }, Options)
            },
            new object[]
            {
                new ToSerialize { Id = 3, Name = null, Date = DateTime.Today },
                JSON.Serialize(new ToSerialize { Id = 3, Name = null, Date = DateTime.Today }, Options)
            },
            new object[]
            {
                new ToSerialize { Id = 4, Name = null, Date = null },
                JSON.Serialize(new ToSerialize { Id = 4, Name = null, Date = null }, Options)
            }
        };

        [TestCaseSource("SerializationExamples")]
        public void WeakSerialize_ReturnsCorrectSerialization(object toSerialize, string expectedSerialization)
        {
            // arrange
            var options = Options;
            var sut = new JsonSerializer(options);

            // act
            var actualSerialization = sut.Serialize(toSerialize);

            // assert
            actualSerialization
                .Should()
                .Be(expectedSerialization);
        }

        [TestCaseSource("SerializationExamples")]
        public void StronglyTypedSerialize_ReturnsCorrectSerialization(ToSerialize toSerialize, string expectedSerialization)
        {
            // arrange
            var options = Options;
            var sut = new JsonSerializer(options);

            // act
            var actualSerialization = sut.Serialize(toSerialize);

            // assert
            actualSerialization
                .Should()
                .Be(expectedSerialization);
        }

        [TestCaseSource("SerializationExamples")]
        public void WeakDeserialize_ReturnsCorrectObject(object expectedObject, string toDeserialize)
        {
            // arrange
            var options = Options;
            var typeToDeserialize = expectedObject.GetType();
            var sut = new JsonSerializer(options);

            // act
            var actualObject = sut.Deserialize(typeToDeserialize, toDeserialize);

            // assert
            actualObject
                .ShouldBeEquivalentTo(expectedObject);
        }

        [TestCaseSource("SerializationExamples")]
        public void StronglyTypedDeserialize_ReturnsCorrectObject(ToSerialize expectedObject, string toDeserialize)
        {
            // arrange
            var options = Options;
            var sut = new JsonSerializer(options);

            // act
            var actualObject = sut.Deserialize<ToSerialize>(toDeserialize);

            // assert
            actualObject
                .ShouldBeEquivalentTo(expectedObject);
        }
    }
}
