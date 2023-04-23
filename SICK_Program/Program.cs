using Sick_test;


namespace SickScanner
{
/*



*/
    class Program
    {
        static void Main(){
            var returns = new Sick_test.returns();
            Sick_test.SickScanners.RunScanners(returns);
        }

        // dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained=true -p:GenerateRuntimeConfigurationFiles=true -p:IncludeNativeLibrariesForSelfExtract=true
    
        //sudo cp SICK.service /etc/systemd/system/
        //sudo systemctl daemon-reload
        //sudo systemctl status SICK.service


        //sudo cp -r publish/ /usr/sbin/
        //sudo chmod 777 --recursive /usr/sbin/publish/
        //sudo mv /usr/sbin/publish /usr/sbin/SICK_Program

        //sudo cp config.json /
    }
}