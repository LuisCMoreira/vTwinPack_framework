﻿using dataTomqttExtractor;

var modbusGet =new  modbusTCPget.ModbusHandler("modbusTCP");

var DAgetter = new DAgent.opcDAget("OPCDA");
var mPUB = new Magent.mqttPUB("MQTT");
//var certt = new MQTTNet_AWS.awsCertConvertion("certs");
var config = new jsonReadSpace.JConfig("agentConfig.json");



var taskRate = 1000;

taskRate = setupFromJSON(config, ref jsonImport.deviceType, ref jsonImport.deviceID, ref jsonImport.manufacturerName, ref jsonImport.serialNo, ref jsonImport.mqttIP, ref jsonImport.mqttPort, ref jsonImport.mqttClientID,ref jsonImport.mqttBeaconMsg, ref jsonImport.mqttUseCert, ref jsonImport.opcdaVars, ref jsonImport.opcdaIP, ref jsonImport.opcdaServer, ref jsonImport.modbusIP, ref jsonImport.modbusPort, ref jsonImport.modbusVars);


//var certt = new MQTTNet_AWS.awsCertConvertion("certs");

//var mqttcert = certt.certTask();

//mPUB.startMQTTclient(mqttIP, mqttPort, mqttClientID, mqttUseCert, mqttcert);

mPUB.startMQTTclient(jsonImport.mqttIP, jsonImport.mqttPort, jsonImport.mqttClientID);

var daClient = DAgetter.DAclient(jsonImport.opcdaIP, jsonImport.opcdaServer, jsonImport.opcdaVars);

var mbClient= modbusGet.MBclient("localhost", 502);





while (true)
{
    DAgetter.opcdaGet(daClient);


Console.WriteLine($"...{DAgetter.outget}...");
    //mPUB.pubTopic(DAgetter.outget, deviceType, deviceID, manufacturerName, serialNo,  mqttBeaconMsg);

   Thread.Sleep(taskRate);



    // Get Modbus data
 
    List<object[]>? modbusData = modbusGet.ModbusTCPget( mbClient, jsonImport.modbusVars);

    mPUB.pubTopic(modbusData, jsonImport.deviceType, jsonImport.deviceID, jsonImport.manufacturerName, jsonImport.serialNo,  jsonImport.mqttBeaconMsg);

    

    // Process or use the Modbus data as needed
    // For example, you can iterate through the list and do something with each entry
if (modbusData!=null)
{
    foreach (var entry in modbusData)
    {
      
        // Access the Modbus data (entry[0] is the register address, entry[1] is the data value)
       
        string register = (string)entry[0];
        string dataValue = (string)entry[1];
        

        // Your logic to process or use the Modbus data goes here
        // ...

        // For demonstration, you can print the data to the console
        Console.WriteLine($"{register}: {dataValue}");
    }
}

}


static int setupFromJSON(jsonReadSpace.JConfig config, ref string deviceType,ref string deviceID,ref string manufacturerName,ref string serialNo, ref string mqttIP, ref int mqttPort, ref string mqttClientID, ref string mqttBeaconMsg, ref bool mqttUseCert, ref List<string> opcdaVars, ref string opcdaIP, ref string opcdaServer, ref string modbusIP, ref int modbusPort, ref List<object>? modbusVars)
{
    int taskRate;
    Console.Clear();

    config.Getinfo("agentConfig.json");

    deviceType = config.AgentconfigVars.DeviceType;

    deviceID = config.AgentconfigVars.DeviceID;

    manufacturerName = config.AgentconfigVars.ManufacturerName;

    serialNo  = config.AgentconfigVars.serialNo;

    taskRate = config.AgentconfigVars.TimeInMillis;

    if (config.AgentconfigVars.mqtt != null)
    {
        mqttIP = config.AgentconfigVars.mqtt.IP_address;
        mqttPort = config.AgentconfigVars.mqtt.port;
        mqttBeaconMsg = config.AgentconfigVars.mqtt.beaconMsg;
        mqttUseCert = config.AgentconfigVars.mqtt.useCerts;
    }


    if (config.AgentconfigVars.opc_da != null)
    {
        opcdaIP = config.AgentconfigVars.opc_da.IP_address;
        opcdaServer = config.AgentconfigVars.opc_da.server;
        if (config.AgentconfigVars.opc_da.variablesToGet != null)
        {
            opcdaVars = config.AgentconfigVars.opc_da.variablesToGet;
        }
    }


    if (config.AgentconfigVars.modbus != null)
    {
        modbusIP = config.AgentconfigVars.modbus.IP_address;
        modbusPort = config.AgentconfigVars.modbus.port;
        if (config.AgentconfigVars.modbus.variablesToGet != null)
        {
        
         modbusVars = new List<object>
         {

         };


        foreach (var modbusVTG in config.AgentconfigVars.modbus.variablesToGet)
        {
            modbusVars.Add(new { modbusVTG.TypeOf, 
                modbusVTG.Address, 
                modbusVTG.Start, 
                modbusVTG.varName  });

        };







        }
    }


    Console.WriteLine(" \n press any key to start \n ");
    Console.ReadKey();
    Console.Clear();
    return taskRate;
}

  