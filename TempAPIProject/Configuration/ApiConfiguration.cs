using System.Linq.Expressions;
using System.Reflection;

namespace TempAPIProject.Configuration;

public static class ApiConfiguration
{
    public static WebApplication ConfigureApi(this WebApplication app)
    {
        app.MapGet("vokurky", GetVokurky);
        return app;
    }

    private static IResult GetVokurky()
    {
        var kosik = new Kosik();
        var result = kosik.ApiResponse(() =>
        {
            var result = kosik.GetPapriksRange(10, 27);
            return result;
        });
        return result;
    }
    private class Kosik
    {
        public List<Papriky> Paprikies { get; set; } = new List<Papriky>();
        public Kosik()
        {
            for (int i = 0; i < 50; i++)
            {
                Paprikies.Add(new Papriky($"klieck {i}", $"value{i}", i));
            }
        }
        public List<Papriky>? GetPapriksRange(int from, int to)
        {
            var result = CallMethodWithCatchBlockAndReturnData<IEnumerable<Papriky>>(() =>
            {
                var a = Paprikies.Where(a => a.counter >= from && a.counter <= to);
                return a;
            }).ToList();
            return result;
        }
        public List<Papriky> GetPapriksRangee(int from, int to)
        {
            List<Papriky> ll = new List<Papriky>();
            CallMethodWithCatchBlock(() =>
            {
                ll = Paprikies.Where(x => x.counter >= from && x.counter <= to).ToList();
            });
            return ll;
        }

        public TOut CallMethodWithCatchBlockAndReturnData<TOut>(Func<TOut> method) 
        {
            try
            {
                return method();
            }
            catch (TimeoutException te)
            {
                throw te;
            }
            catch (AggregateException ex)
            {
                throw new AggregateException();
            }
            catch
            {
                throw new Exception();
            }
        }
        public IResult ApiResponse<TResult>(Func<TResult> method)
        {
            try
            {
                var result =  Results.Ok(method());
                return result;
            }
            catch (TimeoutException te)
            {
                return Results.Problem(te.Message, null, 503, null, null, null);
            }
            catch (AggregateException ex)
            {
                return Results.Problem(ex.Message, null, 408, null, null, null);
            }
            catch ( Exception ex)
            {
                return Results.Problem(ex.Message, null, 500, null, null, null);
            }
        }
        public void CallMethodWithCatchBlock(Action method)
        {
            try
            {
                method();
            }
            catch (TimeoutException te)
            {
                throw te;
            }
            catch (AggregateException ex)
            {
                throw new AggregateException();
            }
            catch
            {
                throw new Exception();
            }
        }
    }
    private record Papriky(string key, string value, int counter);
}
