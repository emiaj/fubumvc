using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuMVC.Core.UI;
using HtmlTags;

namespace FubuMVC.HelloWorld.Controllers.Bindings
{
    public class BindingsReportController
    {
        private readonly IServiceLocator _services;
        private readonly IObjectResolver _resolver;
        private readonly IRequestData _data;
        public BindingsReportController(IServiceLocator services, IObjectResolver resolver, IRequestData data)
        {
            _services = services;
            _resolver = resolver;
            _data = data;
        }

        public HtmlDocument get_Form(BindingReportFormInput input)
        {
            var document = new FubuHtmlDocument<BindingReportFormInput>(_services) {Model = new BindingReportFormInput()};
            var form = document.FormFor(document.Model);
            form.Append(document.InputFor(x => x.Foo.Number).Attr("value", "abc").Attr("type", "hidden"));
            form.Add("input", e => e.Attr("type", "submit"));
            document.Add(form);
            document.EndForm();
            return document;
        }

        public HtmlDocument post_Form(BindingReportFormInput input)
        {
            var history = new InMemoryBindingHistory();
            var context = new BindingContext(_data, _services, new RecordingBindingLogger(history));
            var result = _resolver.BindModel(typeof(BindingReportFormInput), context);
            var report = history.AllReports.Single();
            var document = new FubuHtmlDocument(_services);

            var property = report.For<BindingReportFormInput>(x => x.Foo);

            if(property.Nested == null)
            {
                var message = "Property {0} had problems <div>{1}</div> but the binding report wasn't populated correctly.";
                message = message.ToFormat(property.Property.Name,result.Problems.Select(x => x.ExceptionText).Join("<br/>"));
                document.Add("span").Encoded(false).Text(message);
            }
            else
            {
                var message = "All good: {0}";
                message = message.ToFormat(property.Nested.Properties.SelectMany(x => x.Values).Select(x => x.RawValue.ToString()).Join("<br/>"));
                document.Add("span").Text(message);
            }
            document.Add("br");
            document.Add(document.LinkTo<BindingsReportController>(x => x.get_Form(new BindingReportFormInput())).Text("Back"));
            return document;
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