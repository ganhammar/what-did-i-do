using Amazon.CDK;

namespace AppStack;

public class Program
{
  static void Main(string[] args)
  {
    var app = new App(null);

    new AppStack(app, "what-did-i-do-stack", new StackProps
    {
      Env = new Amazon.CDK.Environment
      {
        Region = "eu-north-1",
      },
    });

    app.Synth();
  }
}
