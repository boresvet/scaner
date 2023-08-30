pipeline {
    agent {
        label 'Ubuntu slave'
    }

    tools {
        dotnetsdk "DotNet6"
    }

    environment {
        MSBUILD_SQ_SCANNER_HOME = tool name: 'SonarNet', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'
    }

    stages {
        
        stage('Build') {
            steps {
                dotnetClean sdk: 'DotNet6'
                dotnetPublish project: 'SICK_Program', configuration: 'Release', properties: [PublishSingleFile: 'true', GenerateRuntimeConfigurationFiles : 'true' , IncludeNativeLibrariesForSelfExtract: 'true'], outputDirectory: "build", runtime: 'linux-x64', sdk: 'DotNet6', selfContained: true
            }
            post {
                success {
                    tar archive: true, compress: true, dir: 'build', file: 'app.tar.gz', overwrite: true
                }
            }
        }

        stage('Sonar') {
            steps {
                withSonarQubeEnv('MainSonar') {
                    sh 'dotnet restore'
                    sh ("""dotnet ${MSBUILD_SQ_SCANNER_HOME}/SonarScanner.MSBuild.dll begin /k:'overal-dimensions-2'""")
                    sh "dotnet build Sick-test.sln"
                    sh "dotnet ${MSBUILD_SQ_SCANNER_HOME}/SonarScanner.MSBuild.dll end"
                }
            }
        }

        stage('Deploy test server') {
            steps {
                 sshPublisher(
                    failOnError: false,
                    publishers: [
                        sshPublisherDesc(
                            configName: "Test-81-160",
                            transfers: [
                                sshTransfer(
                                    sourceFiles: 'build/Sick-test',
                                    removePrefix: 'build',
                                    remoteDirectory: 'GabaritApp'
                                ),
                                sshTransfer(execCommand: 'sudo systemctl stop gabarit'),
                                sshTransfer(execCommand: 'rm -Rf /var/gabarit/Gabarit*'),
                                sshTransfer(execCommand: 'cp GabaritApp/Gabarit* /var/gabarit'),
                                sshTransfer(execCommand: 'chmod a+x /var/gabarit/Gabarit'),
                                sshTransfer(execCommand: 'rm -Rf GabaritApp'),
                                sshTransfer(execCommand: 'sudo systemctl start gabarit')
                            ],
                            sshRetry: [
                                retries: 0
                            ]
                        )
                    ]
                )
            }    
        }
       
    }
}