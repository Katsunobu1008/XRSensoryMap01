import Foundation
import CoreMotion

public class PressureSensor {
    private let altimeter = CMAltimeter()
    private var currentPressure = 0.0   // [hPa]

    public func getCurrentAtmosphericPressure() -> Double {
        return self.currentPressure
    }

    public func start() -> Bool {
        if CMAltimeter.isRelativeAltitudeAvailable() {
            altimeter.startRelativeAltitudeUpdates(
                to: OperationQueue.main, 
                withHandler: { data, error in
                    if error == nil && data != nil {
                        self.currentPressure = data!.pressure.doubleValue * 10.0
                    }
                }
            )
            return true
        }
        else {
            return false
        }
    }

    public func stop() {
        altimeter.stopRelativeAltitudeUpdates()
        self.currentPressure = 0.0
    }
}

@_cdecl("createPressureSensor")
public func createPressureSensor() -> UnsafeRawPointer {
    let instance = PressureSensor()
    return UnsafeRawPointer(Unmanaged<PressureSensor>.passRetained(instance).toOpaque())
}

@_cdecl("releasePressureSensor")
public func releasePressureSensor(_ pressureSensor: UnsafeRawPointer) {
    Unmanaged<PressureSensor>.fromOpaque(pressureSensor).release()
}

@_cdecl("startAtmosphericPressure")
public func startAtmosphericPressure(_ pressureSensor: UnsafeRawPointer) -> UInt8 {
    let instance = Unmanaged<PressureSensor>.fromOpaque(pressureSensor).takeUnretainedValue()
    return instance.start() ? 1 : 0
}

@_cdecl("stopAtmosphericPressure")
public func stopAtmosphericPressure(_ pressureSensor: UnsafeRawPointer) {
    let instance = Unmanaged<PressureSensor>.fromOpaque(pressureSensor).takeUnretainedValue()
    return instance.stop()
}

@_cdecl("getCurrentAtmosphericPressure")
public func getCurrentAtmosphericPressure(_ pressureSensor: UnsafeRawPointer) -> Double {
    let instance = Unmanaged<PressureSensor>.fromOpaque(pressureSensor).takeUnretainedValue()
    return instance.getCurrentAtmosphericPressure()
}

