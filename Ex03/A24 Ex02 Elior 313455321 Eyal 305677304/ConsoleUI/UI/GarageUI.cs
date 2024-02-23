﻿using ConsoleUI.UI.Enums;
using ConsoleUI.UI.Printer;
using ConsoleUI.UI.Reader;
using ConsoleUI.UI.Verifier;
using GarageLogic.Vehicles.Factory;
using GarageLogic.Vehicles;
using System;
using GarageLogic.Manager;
using GarageLogic.Vehicles.Types.Objects.MotorCycle;
using GarageLogic.Vehicles.Types.Objects.Car;
using GarageLogic.Vehicles.Types.Objects.Truck;
using GarageLogic.Vehicles.Types;
using System.Collections.Generic;

namespace ConsoleUI.UI
{
    internal class GarageUI
    {
        private readonly OutputPrinter r_OutputPrinter = new OutputPrinter();
        private readonly InputReader r_InputReader = new InputReader();
        private readonly InputVerifier r_Verifier = new InputVerifier();
        private readonly VehicleFactory r_VehicleFactory = new VehicleFactory();
        private readonly GarageManager r_GarageManager = new GarageManager();

        public void RunSystem()
        {
            bool hasUserExited = false;
            string userInput;

            r_OutputPrinter.GreetUser();
            
            while (!hasUserExited)
            {
                try
                {
                    userInput = handleActionChoice();
                    r_Verifier.VerifyActionChoice(userInput, out eUserOptions userChoice);
                    ExecuteUserAction(userChoice);
                    hasUserExited = userChoice.Equals(eUserOptions.Exit);
                }

                catch (Exception e)
                {
                    r_OutputPrinter.PrintError(e.Message);
                }
            }
        }

        private string handleActionChoice()
        {
            r_OutputPrinter.PrintUserOptions();
            return r_InputReader.ReadUserChoice();
        }

        public void ExecuteUserAction(eUserOptions i_UserChoice)
        {
            bool successfullyExecuted = false;

            while (!successfullyExecuted)
            {
                try
                {
                    switch (i_UserChoice)
                    {
                        case eUserOptions.InsertVehicle:
                            insertNewVehicle();
                            break;

                        case eUserOptions.DisplayLicenses:
                            displayLicenses();
                            break;

                        case eUserOptions.UpdateVehicleState:
                            updateVehicleState();
                            break;

                        case eUserOptions.InflateVehicleTires:
                            inflateVehicleTires();
                            break;

                        case eUserOptions.FillGas:
                            fillGas();
                            break;

                        case eUserOptions.ChargeBattery:
                            chargeBattery();
                            break;

                        case eUserOptions.DisplayVehicleInfo:
                            displayVehicleInfo();
                            break;

                        case eUserOptions.Exit:
                            break;

                        default:
                            break;
                    }

                    successfullyExecuted = true;
                }

                catch (Exception e)
                {
                    r_OutputPrinter.PrintError($"{e.Message}");
                }
            }
        }

        private void insertNewVehicle()
        {
            bool successfullyInserted = false;

            while (!successfullyInserted)
            {
                try
                {
                    eVehicleType vehicleType = readVehicleType();
                    Vehicle generatedVehicle = r_VehicleFactory.GenerateVehicle(vehicleType);
                    Owner vehicleOwner = readOwnerInfo();
                    VehicleRecord vehicleRecord = new VehicleRecord();
                    vehicleRecord.Owner = vehicleOwner;
                    vehicleRecord.Status = eVehicleStatus.Fixing;
                    readVehicleInfo(generatedVehicle, vehicleType);
                    r_GarageManager.InsertNewVehicle(generatedVehicle, vehicleRecord);
                    Console.WriteLine("Successfully Inserted!");
                    successfullyInserted = true;
                }

                catch (ArgumentException)
                {
                    r_OutputPrinter.PrintError("Invalid choice!");
                }

                catch (Exception e)
                {
                    r_OutputPrinter.PrintError(e.Message);
                }
            }
        }

        private eVehicleType readVehicleType()
        {
            r_OutputPrinter.PrintVehicleOptions();
            string userInput = r_InputReader.ReadUserChoice();
            r_Verifier.VerifyVehicleTypeChoice(userInput, out eVehicleType userChoice);

            return userChoice;
        }

        private void readVehicleInfo(Vehicle i_Vehicle, eVehicleType i_VehicleType)
        {
            bool successfullyRead = false;

            while (!successfullyRead)
            {
                try
                {
                    handleVehicleData(i_Vehicle);
                    handleWheelData(i_Vehicle, i_VehicleType);
                    handleUniqueVehicleTypeData(i_Vehicle, i_VehicleType);
                    successfullyRead = true;
                }

                catch (Exception e)
                {
                    r_OutputPrinter.PrintError($"{e.Message}");
                }
            }
        }

        private void handleVehicleData(Vehicle i_Vehicle)
        {
            bool successfullyRead = false;

            while (!successfullyRead)
            {
                try
                {
                    readVehicleData(out string modelInput, out string licenseInput, out string energyInput);
                    r_Verifier.VerifyEnergyPercentageInput(energyInput, out float energyPercentage);
                    insertVehicleInfo(i_Vehicle, modelInput, licenseInput, energyPercentage);
                    successfullyRead = true;
                }

                catch(Exception e)
                {
                    r_OutputPrinter.PrintError(e.Message);
                }
            }
        }

        private void handleWheelData(Vehicle i_Vehicle, eVehicleType i_VehicleType)
        {
            bool successfullyRead = false;

            while (!successfullyRead)
            {
                try
                {
                    readWheelData(out string manufacturorName, out string airPressureInput);
                    r_Verifier.VerifyAirPressureInput(airPressureInput, out float airPressure, i_VehicleType);
                    insertWheelInfo(i_Vehicle, manufacturorName, airPressure);
                    successfullyRead = true;
                }

                catch (Exception e)
                {
                    r_OutputPrinter.PrintError(e.Message);
                }
            }
        }

        private Owner readOwnerInfo()
        {
            Owner owner = new Owner();

            owner.Name = r_InputReader.ReadOwnerName();
            owner.Phone = r_InputReader.ReadOwnerPhone();

            return owner;
        }

        private void readVehicleData(out string o_Model, out string o_License, out string o_Energy)
        {
            o_Model = r_InputReader.ReadVehicleModel();
            o_License = r_InputReader.ReadLicensePlate();
            o_Energy = r_InputReader.ReadEnergyPercentage();
        }

        private void readWheelData(out string o_ManufacturorName, out string o_CurrentAirPressure)
        {
            o_ManufacturorName = r_InputReader.ReadWheelManufacturorName();
            o_CurrentAirPressure = r_InputReader.ReadWheelCurrentAirPressure();
        }

        private void insertVehicleInfo(Vehicle i_Vehicle, string i_Model, string i_License, float i_EnergyPercentage)
        {
            i_Vehicle.VehicleInfo.RemainingEnergyPercentage = i_EnergyPercentage;
            i_Vehicle.VehicleInfo.Model = i_Model;
            i_Vehicle.VehicleInfo.LicenseID = i_License;
        }

        private void insertWheelInfo(Vehicle i_Vehicle, string i_ManufacturorName, float i_CurrentAirPressure)
        {
            foreach (Wheel wheel in i_Vehicle.Wheels)
            {
                wheel.ManufacturerName = i_ManufacturorName;
                wheel.CurrentAirPressure = i_CurrentAirPressure;
            }
        }

        private void handleUniqueVehicleTypeData(Vehicle i_Vehicle, eVehicleType i_VehicleType)
        {
            bool successfullyRead = false;

            while (!successfullyRead)
            {
                try
                {
                    switch (i_VehicleType)
                    {
                        case eVehicleType.FueledMotoryCycle:
                        case eVehicleType.ElectricalMotorCycle:
                            readMotorCycleData(i_Vehicle);
                            break;

                        case eVehicleType.FueledCar:
                        case eVehicleType.ElectricalCar:
                            readCarData(i_Vehicle);
                            break;

                        case eVehicleType.Truck:
                            readTruckData(i_Vehicle);
                            break;

                        default:
                            break;
                    }

                    successfullyRead = true;
                }

                catch (Exception e)
                {
                    r_OutputPrinter.PrintError(e.Message);
                }
            }
        }

        private void readMotorCycleData(Vehicle i_MotorCycle)
        {
            MotorCycleInfo motorCycleInfo = new MotorCycleInfo();

            motorCycleInfo.MotorCycleLicense = readMotorCycleLicense();
            motorCycleInfo.EngineVolume = readMotorCycleEngineVolume();
            insertMotorCycleInfo(i_MotorCycle, motorCycleInfo);
        }

        private eMotorCycleLicense readMotorCycleLicense()
        {
            r_OutputPrinter.PrintSupportedMotorCycleLicenses();
            string licenseInput = r_InputReader.ReadMotorCycleLicense();
            r_Verifier.VerifyMotorCycleLicenseInput(licenseInput, out eMotorCycleLicense motorCycleLicense);

            return motorCycleLicense;
        }

        private int readMotorCycleEngineVolume()
        {
            string engineVolumeInput = r_InputReader.ReadMotorCycleEngineVolume();
            r_Verifier.VerifyMotorCycleEngineInput(engineVolumeInput, out int engineVolume);

            return engineVolume;
        }

        private void insertMotorCycleInfo(Vehicle i_MotorCycle, MotorCycleInfo i_MotorCycleInfo)
        {
            if (i_MotorCycle is FueledMotorCycle)
            {
                FueledMotorCycle fueledMotorCycle = i_MotorCycle as FueledMotorCycle;

                fueledMotorCycle.MotorCycleInfo = i_MotorCycleInfo;
                handleFuelData(fueledMotorCycle);
            }

            else
            {
                ElectricalMotorCycle electricalMotorCycle = i_MotorCycle as ElectricalMotorCycle;

                electricalMotorCycle.MotorCycleInfo = i_MotorCycleInfo;
                handleBatteryData(electricalMotorCycle);
            }
        }

        private void readCarData(Vehicle i_Car)
        {
            CarInfo carInfo = new CarInfo();

            carInfo.Color = readCarColor();
            carInfo.NumOfDoors = readCarNumOfDoors();
            insertCarData(i_Car, carInfo);
        }

        private eCarColors readCarColor()
        {
            r_OutputPrinter.PrintSupportedCarColors();
            string colorInput = r_InputReader.ReadCarColor();
            r_Verifier.VerifyCarColorInput(colorInput, out eCarColors carColor);

            return carColor;
        }

        private eCarDoors readCarNumOfDoors()
        {
            r_OutputPrinter.PrintSupportedNumOfCarDoors();
            string doorsInput = r_InputReader.ReadNumOfCarDoors();
            r_Verifier.VerifyNumOfCarDoorsInput(doorsInput, out eCarDoors carDoors);

            return carDoors;
        }

        private void insertCarData(Vehicle i_Car, CarInfo i_CarInfo)
        {
            if (i_Car is FueledCar)
            {
                FueledCar fueledCar = i_Car as FueledCar;

                fueledCar.CarInfo = i_CarInfo;
                handleFuelData(fueledCar);
            }

            else
            {
                ElectricalCar electricalCar = i_Car as ElectricalCar;

                electricalCar.CarInfo = i_CarInfo;
                handleBatteryData(electricalCar);
            }
        }

        private void readTruckData(Vehicle i_Truck)
        {
            TruckInfo truckInfo = new TruckInfo();
            Truck truck = i_Truck as Truck;

            truckInfo.HasDangerousLuggage = readDangerousLuggageData();
            truckInfo.LuggageCapacity = readLuggageCapacity();
            truck.TruckInfo = truckInfo;
            handleFuelData(truck);
        }

        private bool readDangerousLuggageData()
        {
            string dangerousLuggageInput = r_InputReader.ReadDangerousLuggageInfo();
            r_Verifier.VerifyDangerousLuggageInput(dangerousLuggageInput, out bool hasDangerousLuggage);

            return hasDangerousLuggage;
        }

        private float readLuggageCapacity()
        {
            string luggageCapacityInput = r_InputReader.ReadLuggageCapacity();
            r_Verifier.VerifyLuggageCapacity(luggageCapacityInput, out float luggageCapacity);

            return luggageCapacity;
        }

        private void handleFuelData(FueledVehicle i_FueledVehicle)
        {
            i_FueledVehicle.RemainingFuel = (i_FueledVehicle.MaxFuelCapacity * 
                i_FueledVehicle.VehicleInfo.RemainingEnergyPercentage) / 100;
        }

        private void handleBatteryData(ElectricalVehicle i_ElectricalVehicle)
        {
            i_ElectricalVehicle.RemainingBatteryTime = (i_ElectricalVehicle.MaxBatteryTime *
                i_ElectricalVehicle.VehicleInfo.RemainingEnergyPercentage) / 100;
        }

        private void displayLicenses()
        {
            r_OutputPrinter.PrintVehicleStatuses();
            string vehicleStatus = r_InputReader.ReadVehicleStatus();
            List<string> licenses = r_GarageManager.FilterLicensePlatesByStatus(vehicleStatus);
            r_OutputPrinter.PrintLicensePlates(vehicleStatus, licenses);
        }

        private void updateVehicleState()
        {
            string licensePlate = r_InputReader.ReadLicensePlate();
            string newState = r_InputReader.ReadNewVehicleState();
            string oldState = r_GarageManager.UpdateVehicleState(licensePlate, newState);

            Console.WriteLine($"Successfully updated vehicle licensed {licensePlate}'s state from {oldState} to {newState}!");
        }

        private void inflateVehicleTires()
        {
            string licensePlate = r_InputReader.ReadLicensePlate();

            r_GarageManager.InflateVehicleTiresToMaximum(licensePlate);
            Console.WriteLine($"Successfully inflated vehicle licensed {licensePlate}'s wheels!");
        }

        private void fillGas()
        {
            string licensePlate = r_InputReader.ReadLicensePlate(), fuelType, fuelAmount;

            r_OutputPrinter.PrintFuelTypes();
            fuelType = r_InputReader.ReadFuelType();
            fuelAmount = r_InputReader.ReadFuelAmoint();

        }

        private void chargeBattery()
        {

        }

        private void displayVehicleInfo()
        {

        }
    }
}