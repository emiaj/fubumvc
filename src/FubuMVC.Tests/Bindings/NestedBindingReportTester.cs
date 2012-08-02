using System.Linq;
using FubuCore.Binding.InMemory;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Bindings
{
    [TestFixture]
    public class NestedBindingReportTester
    {
        private BindingScenario<BindingReportFormInput> _scenario;

        [SetUp]
        public void Setup()
        {
            _scenario = BindingScenario<BindingReportFormInput>
                .For(e => e.Data("FooNumber","abc"));
        }

        [Test]
        public void it_should_have_binding_problems()
        {
            _scenario.Problems.ShouldHaveCount(1)
                .First().Property.Name.ShouldEqual("Number");
        }

        [Test]
        public void it_should_report_the_nested_property_binding()
        {
            _scenario.Report.Properties.ShouldHaveCount(1)
                .First().Nested.ShouldNotBeNull()
                .Properties.ShouldHaveCount(1)
                .First().Property.Name.ShouldEqual("Number");
        }
    }


    public class BindingReportFormInput
    {
        public Foo Foo { get; set; }
    }

    public class Foo
    {
        public int Number { get; set; }
    }
}