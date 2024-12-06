using MediatorForge.CQRS.Commands;
using ResultifyCore;

namespace MediatorForge.Adapters.Tests.Requests;
public class TestRequest : ICommand<TestResponse>
{
    public string RequestData { get; set; }
}

public class TestRequestOption : ICommand<Option<TestResponse>>
{
    public string RequestData { get; set; }
}

public class TestRequestResult : ICommand<Result<TestResponse>>
{
    public string RequestData { get; set; }
}
