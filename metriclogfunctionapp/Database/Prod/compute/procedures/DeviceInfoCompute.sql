CREATE OR REPLACE PROCEDURE compute.DeviceInfoCompute(
    "startDate" timestamp with time zone,
    "endDate" timestamp with time zone)
AS $$
BEGIN
    INSERT INTO compute."DeviceInfo"
    SELECT cte.*, count(*) "Sessions" FROM
        (
            SELECT
                dev."Timestamp"::date "Date",
                COALESCE(TRIM(dev."AppVersion"), 'Unknown') AS "AppVersion",
                COALESCE(TRIM(dev."DeviceManufacturer"), 'Unknown') AS "DeviceManufacturer",
                CASE WHEN ios."Generation" IS NOT NULL THEN COALESCE(TRIM(ios."Generation"), 'Unknown') ELSE COALESCE(TRIM(dev."DeviceModel"), 'Unknown') END AS "DeviceModel",
                COALESCE(TRIM(dev."DeviceOS"), 'Unknown') AS "DeviceOS",
                COALESCE(TRIM(dev."DeviceOSVersion"), 'Unknown') AS "DeviceOSVersion",
                CASE
                    WHEN "AppVersion" IS NULL THEN CASE
                        WHEN LOWER(dev."UserAgent") SIMILAR TO '%(android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini)%' THEN 'Mobile browser'
                        ELSE 'Browser'
                    END
                    ELSE 'Native'
                END AS "Access"
            FROM "events"."Device" dev
                     LEFT JOIN
                 (
                     SELECT DISTINCT
                         "Generation" ,
                         "Identifier"
                     FROM "ref"."ios_device_identifier" ios
                 ) AS ios ON ios."Identifier" = REPLACE(dev."DeviceModel",CHR(46),CHR(44))
            WHERE dev."Timestamp" >= "startDate" AND dev."Timestamp" < "endDate"
        ) AS cte
    GROUP BY "Date", "AppVersion", "DeviceManufacturer", "DeviceModel", "DeviceOS", "DeviceOSVersion", "Access"
    on conflict on constraint dupes_pkey do update
        SET "Date" = Excluded."Date",
            "AppVersion" = Excluded."AppVersion",
            "DeviceManufacturer" = Excluded."DeviceManufacturer",
            "DeviceModel" = Excluded."DeviceModel",
            "DeviceOS" = Excluded."DeviceOS",
            "DeviceOSVersion" = Excluded."DeviceOSVersion",
            "Access" = Excluded."Access",
            "Sessions" = Excluded."Sessions";
END;
$$ LANGUAGE plpgsql;