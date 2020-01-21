﻿// The MIT License (MIT)
// 
// Copyright (c) 2015-2018 Rasmus Mikkelsen
// Copyright (c) 2015-2018 eBay Software Foundation
// https://github.com/eventflow/EventFlow
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.TestHelpers;
using EventFlow.TestHelpers.Aggregates;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace EventFlow.Tests.IntegrationTests.Aggregates
{
    [TestFixture]
    [Category(Categories.Integration)]
    public class AggregateFactoryTests : IntegrationTest
    {
        [Test]
        public async Task CreatesNewAggregateWithIdParameter()
        {
            // Arrange
            var id = ThingyId.New;

            // Act
            var aggregateWithIdParameter = await AggregateFactory.CreateNewAggregateAsync<TestAggregate, ThingyId>(id).ConfigureAwait(false);

            // Assert
            aggregateWithIdParameter.Id.Should().Be(id);
        }

        [Test]
        public async Task CreatesNewAggregateWithIdAndInterfaceParameters()
        {
            // Act
            var aggregateWithIdAndInterfaceParameters = await AggregateFactory.CreateNewAggregateAsync<TestAggregateWithResolver, ThingyId>(ThingyId.New).ConfigureAwait(false);

            // Assert
            aggregateWithIdAndInterfaceParameters.ServiceProvider.Should()
                .NotBeNull().And.BeAssignableTo<IServiceProvider>();
        }

        [Test]
        public async Task CreatesNewAggregateWithIdAndTypeParameters()
        {
            // Act
            var aggregateWithIdAndTypeParameters = await AggregateFactory.CreateNewAggregateAsync<TestAggregateWithPinger, ThingyId>(ThingyId.New).ConfigureAwait(false);

            // Assert
            aggregateWithIdAndTypeParameters.Pinger.Should().BeOfType<Pinger>();
        }

        protected override IEventFlowBuilder Options(IEventFlowBuilder eventFlowBuilder)
        {
            return base.Options(eventFlowBuilder)
                .RegisterServices(c => c.AddTransient(typeof(Pinger)));
        }

        public class Pinger
        {
        }

        public class TestAggregate : AggregateRoot<TestAggregate, ThingyId>
        {
            public TestAggregate(ThingyId id)
                : base(id)
            {
            }
        }

        public class TestAggregateWithPinger : AggregateRoot<TestAggregateWithPinger, ThingyId>
        {
            public TestAggregateWithPinger(ThingyId id, Pinger pinger)
                : base(id)
            {
                Pinger = pinger;
            }

            public Pinger Pinger { get; }
        }

        public class TestAggregateWithResolver : AggregateRoot<TestAggregateWithResolver, ThingyId>
        {
            public TestAggregateWithResolver(ThingyId id, IServiceProvider serviceProvider)
                : base(id)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get; }
        }
    }
}