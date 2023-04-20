using Sick_test;


namespace SickScanner
{

    class Program
    {
        static void Main(){
            var returns = new Sick_test.returns();
            Sick_test.SickScanners.RunScanners(returns);
        }

        // dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained=true -p:GenerateRuntimeConfigurationFiles=true -p:IncludeNativeLibrariesForSelfExtract=true
    }
}