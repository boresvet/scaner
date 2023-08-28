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
                dotnetBuild configuration: 'Release', sdk: 'DotNet6'
            }
            post {
                success {
                    tar archive: true, compress: true, dir: 'Gabarit/bin/Release/net6.0/linux-x64/publish', file: 'app.tar.gz', overwrite: true
                }
            }
        }

        stage('Sonar') {
            steps {
                withSonarQubeEnv('MainSonar') {
                    sh 'dotnet restore'
                    sh ("""dotnet ${MSBUILD_SQ_SCANNER_HOME}/SonarScanner.MSBuild.dll begin /k:'overall-dimensions'""")
                    sh "dotnet build GabaritWebConfig.sln"
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
                            configName: "Test-81-42",
                            transfers: [
                                sshTransfer(
                                    sourceFiles: 'Gabarit/bin/Release/net6.0/linux-x64/publish/**/*',
                                    removePrefix: 'Gabarit/bin/Release/net6.0/linux-x64/publish',
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