INFO_PLIST="${SRCROOT}/${INFOPLIST_FILE}"
PLISTBUDDY="/usr/libexec/PlistBuddy"

if  [ "${CONFIGURATION}" = "Development" ] || [ "${CONFIGURATION}" = "Debug" ] || [ "${CONFIGURATION}" = "BrowserStack" ]|| [ "${CONFIGURATION}" = "BrowserstackLocal" ]; then
    $PLISTBUDDY -c "Set :NSAppTransportSecurity:NSAllowsArbitraryLoads true" "${INFO_PLIST}"
else
    $PLISTBUDDY -c "Set :NSAppTransportSecurity:NSAllowsArbitraryLoads false" "${INFO_PLIST}"
fi

ALLOWS_ARBITRARY_LOADS_ENABLED=$($PLISTBUDDY -c "Print :NSAppTransportSecurity:NSAllowsArbitraryLoads" "${INFO_PLIST}")

if [ "${ALLOWS_ARBITRARY_LOADS_ENABLED}" == "true" ]; then
    echo "warning: ATS is disabled. :NSAppTransportSecurity:NSAllowsArbitraryLoads set to true. This is insecure and should not be used in release builds."
else
    echo "information: ATS is enabled. :NSAppTransportSecurity:NSAllowsArbitraryLoads set to false"
fi
