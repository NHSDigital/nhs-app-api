CREATE OR REPLACE PROCEDURE events.DeviceInsert (
    deviceTimestamp timestamp with time zone,
    sessionId character varying,
    appVersion character varying,
    deviceManufacturer character varying,
    deviceModel	character varying,
    deviceOS character varying,
    deviceOSVersion	character varying,
    userAgent text,
    auditId character varying(36))
AS $$

BEGIN
    INSERT INTO events."Device" (
        "Timestamp",
        "SessionId",
        "AppVersion",
        "DeviceManufacturer",
        "DeviceModel",
        "DeviceOS",
        "DeviceOSVersion",
        "UserAgent",
        "AuditId"
        )
    VALUES 
    (
        deviceTimestamp,
        sessionId,
        appVersion,
        deviceManufacturer,
        deviceModel,
        deviceOS,
        deviceOSVersion,
        userAgent,
        auditId
        )
    ON CONFLICT ON CONSTRAINT device_timestamp_sessionid_auditid_unique
    DO NOTHING;
END;

$$ LANGUAGE plpgsql;