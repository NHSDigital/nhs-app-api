package mocking.stubs

import config.Config
import models.Patient
import models.patients.EmisPatients
import worker.models.session.UserSessionRequest

class EmisStubsPatientFactory {
    companion object {
        private val CONNECTION_TOKEN_SUFFIX_LENGTH = 12
        private val CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH = 2

        val goodPatientEMIS = generatePatientData(
               PatientUniqueId.GoodPatientEMIS, "goodpatient", CONNECTION_TOKEN_SUFFIX_LENGTH, GOOD_TOKEN)

        val timeoutPatientEMIS = generatePatientData(
                PatientUniqueId.TimeoutPatientEMIS, "timeoutpatient", CONNECTION_TOKEN_SUFFIX_LENGTH,
                TIMEOUT_TOKEN)

        val serviceNotEnabledPatientEMIS = generatePatientData(
                PatientUniqueId.ServiceNotEnabledPatientEMIS, "servicennotenabledpatient",
                CONNECTION_TOKEN_SUFFIX_LENGTH, SERVICE_NOT_ENABLED_TOKEN)

        val sessionErrorPatientEMIS = generatePatientData(
                PatientUniqueId.SessionErrorPatientEMIS, "sessionerrorpatient",
                CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH, SESSION_ERROR_TOKEN)

        val serverErrorPatientEMIS = generatePatientData(
                PatientUniqueId.ServerErrorPatientEMIS, "servererrorpatient",
                CONNECTION_TOKEN_SUFFIX_LENGTH, SERVER_ERROR_TOKEN)

        val EMISPatientList = listOf(
                goodPatientEMIS,
                serviceNotEnabledPatientEMIS,
                sessionErrorPatientEMIS,
                timeoutPatientEMIS,
                serverErrorPatientEMIS
        )

        private fun generatePatientData(uniqueId: PatientUniqueId, loginID: String, length: Int, accessToken: String):
                Patient {
            val pad = uniqueId.Id.padStart(length, '0')
            //do not add end user session id here
            val patient = EmisPatients.picaJones.copy(
                    firstName = "You are logged in as",
                    surname = loginID,
                    cidUserSession = UserSessionRequest(
                            authCode = "authCode$pad",
                            codeVerifier = "codeVerifier$pad",
                            redirectUrl = Config.instance.cidRedirectUri
                    ),
                    connectionToken = "00000000-0000-0000-0000-$pad",
                    userPatientLinkToken = "userPatientLinkToken$pad"
            )
            patient.accessToken = accessToken
            return patient
        }
    }
}

private const val GOOD_TOKEN: String = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQiOi" +
        "JuaHMtb25saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHBzOi" +
        "8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjgyLCJhbG" +
        "ciOiJSUzUxMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy02N" +
        "TkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC5le" +
        "HQuc2lnbmluLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hc" +
        "msvYXV0aC5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI5N" +
        "zQ0MzY2NzUzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OSwic" +
        "2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm90I" +
        "joiUDkuQ3AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4MiwicmVhc29uX2Zvcl9yZXF1ZXN0IjoicGF0aWVud" +
        "GFjY2VzcyIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.n9F1ovjlxNDSqGob7_yrBEYsEID0" +
        "8J4s9eLSjCMYQS_TZO63RHWOlnjG8WrZcDPfH5W8yuMw5XBYwbAWD8DasMXy2Fe2z65LP6eL2-SnutQwWl4InLVu8saIzRhX3L2P" +
        "Uy5kLJNapJacbvSUioJm48ClmJff6T9UABfCMB3N2O-QzLUoH6drkfJbwblZRB8_mV4px768gdAD1Yg-hMW25UgnJ2ExjQPMDP1-" +
        "lM0aNF_Ie-ejG84F8xabLEJ6ZO3Y_-8x7JZgMF0P-4-4g3RMCNd4xWH1QOZ2jX2CiOZ8Mtv_QU7ia_KEJh2yzZyEFbEdYaEO2TpS" +
        "CnxwdDjMwX_RGYN3NDf-5ZymYpf6Vm8PSxRbmT28xGNSH856DnlM8c4cvZ9Nn3rBFt0lJRDIutQfgL63mNk7_VQYY3uXADNoJFJw" +
        "yvZMS9rFWutzjQWV-uRlND3z1YQNsZ6s0Ha4d6dtvDOE6zdyM8soHpiYiuvo9GtLCg9984EYKYP69sHvwLtxb5txpk5HqemowZvF" +
        "76sJIJbl4L8QfMHZ4lpsIKpBY_s4qNYGczb3aUm23coJD0v31_P1peRO67NaFl0_ie7xbosnPVFnPt0p29E9NbORLcl1bB-S9YGo" +
        "kAPIRyAeHkhvu0-w89dkAIXBTWf1vFJeCg7X2MNG7Y_LW8EgtPOLW1s"

private const val TIMEOUT_TOKEN: String = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQ" +
        "iOiJuaHMtb25saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHB" +
        "zOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjgzLCJ" +
        "hbGciOiJSUzUxMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy" +
        "02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC" +
        "5leHQuc2lnbmluLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG" +
        "1hcmsvYXV0aC5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOi" +
        "I5NzQ0MzY2NzUzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OS" +
        "wic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm" +
        "90IjoiUDkuQ3AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4MywicmVhc29uX2Zvcl9yZXF1ZXN0IjoicGF0aW" +
        "VudGFjY2VzcyIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.u6LUd1VIKf5ivvKvs3OoG3LY1" +
        "ip7DNwyUH9xorfoSXVXQSatoqbGDIf5ohEUotALMjI7Hd7gAXAIbeBbUT8XDZtOccJ_tgHllWdM1IlQN7Aw72hjHqqHvA_v2Wx5W" +
        "j0mHJXxS2on5jkU47vJAlwlHrlX5scB7b923-XO8By7b6OC1VFvnHs_SaeYTsix-BcO3BWxN7Rsfaf5mGGKXpkPzEi_qLSdXsZ0f" +
        "wuGeHxAQV417nbNHwCW8ZHxTBQKm1Nd5-uWlnBM7CfrN6bkzDnTxuh1TjL8hUX_1liIDgm9b-IOz-9KWpWdEY7qFWxMS2g-4YuGp" +
        "pohdFlrDmlUMbpNwDYe_YAsZPJZzNpNF_SGLUKfgjzfbC59FBe3Nd-2qa0TIUqorZ6VJNKwNBhxh8HBWGxrPS2yaZ5Ly5FNlStjN" +
        "ruMoI5kEpQV0kHA0FR-KyhFGPuumCNc7uZF9wq_BNJGdtyrRMV8t0_iZqZqaEZf49Im1ND2rqetTCGoZR1RhQ8D63Jlb-8OJSJIU" +
        "gq00UJqhqdbEkLmx3SB9tX3XMREh27B7jgl5FY6glIQIUD_uEmreB36dLihpythu_7r8BUwOV4gcYxy5W0Uvcn_xEDc3YrWDuEr_" +
        "5-4oCdQfK0ZgStt9BegAX4qGq6_FCVcp81tztMwoG3YM40rUfFmIAjc0hs"


private const val SERVICE_NOT_ENABLED_TOKEN: String = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmY" +
        "WQiLCJhdWQiOiJuaHMtb25saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzc" +
        "yI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyM" +
        "zEyMjg0LCJhbGciOiJSUzUxMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0" +
        "NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBz" +
        "Oi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51" +
        "ay90cnVzdG1hcmsvYXV0aC5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3Bh" +
        "dGllbnQiOiI5NzQ0MzY2NzUzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2" +
        "MjMxMjI3OSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRp" +
        "YWxzIiwidm90IjoiUDkuQ3AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4NCwicmVhc29uX2Zvcl9yZXF1ZXN0" +
        "IjoicGF0aWVudGFjY2VzcyIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.o4UdpxpdLLQ7bfZ" +
        "XmSoFv99x0eC15cBqVEpHzjZ9nQcsOPheUnj6su7RWpp3MxuseM1cePvHuss16Dmw_kH_21s_FgPCSFn1WOPjyS_gdx0cuT3BpZI" +
        "kcujJ5ZhQ3fPOWslH7583-1IgGQ6Y5ElC6mwRvXLMnf0aNJ6i4_RwivEZYurnQSnXG7BsmuXpKyxKlLLoXrixoQMHOoAzqUyrxMi" +
        "GaHfKaqVfyCjBEYSiFuikTmldJAZ5VWZ9uR1rs3om5tN0CDdNcwHzlQvmIS5MQ2pP4yjZH2Hy6HuiTWnFQYzTPWo0s1bXH0z_Roi" +
        "NreW7is60q5CoxVZV-EztjlkEQJgF6ZR9uFe6NNGf1LXsID2n3GsVWANk_rvSqYuW8CGRuewAHFeyrQildjdpKwoR8rwE2j5Tjue" +
        "_hefE_BYLRDuCytInfxDw8K4pKKaS7gxQwGYKVZZ8jo4I_-j91neQMV3dfE0D2nWe-9R62LLt4g86K7oWJOzbxb5szEUErmPiB1a" +
        "4IAYVbX8rOWMREM1WOO88ltnHwRMdVhL36f0WRRfwjRFIQ0hjQtvmivmor7tlGHMuv9ErTuETskhbHJAkxWbkQW2VxmWxfl-DUdU" +
        "FVxC-nl5BmwK9Cni6e5fcdoHcWX-0CzgOzZ3ZTRLJADYLRo7RJec4QNWuyf2fnvtV3Gc"


private const val SESSION_ERROR_TOKEN: String = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLC" +
        "JhdWQiOiJuaHMtb25saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Im" +
        "h0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMj" +
        "g1LCJhbGciOiJSUzUxMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmO" +
        "DBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vY" +
        "XV0aC5leHQuc2lnbmluLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90c" +
        "nVzdG1hcmsvYXV0aC5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllb" +
        "nQiOiI5NzQ0MzY2NzUzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxM" +
        "jI3OSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzI" +
        "iwidm90IjoiUDkuQ3AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4NSwicmVhc29uX2Zvcl9yZXF1ZXN0Ijoic" +
        "GF0aWVudGFjY2VzcyIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.SReVcOvo6RuNk5_MJB5_" +
        "NkoYMgmENU8FJpq0t8y4-LLHZpncpyNLoF-Obq7AnOBbty6lX6Plk0tVfLStkjunWArK9RMBIuOF0TWg42RejZZHe2KYklOsSq52" +
        "-RPtDU_JOpy2gw037_Bq8C5JJPuyPPCz7qtNVAKYSMggarPX6Me6WjQBm51fVC2LnCiC5TFlvzrS2Yg6iwZ-cgEu3iXcPWnhtdP2" +
        "NWclIdeSZQAqvHAyZoHGrMFYm5NWpF64r-Mx23gSJmO5e9Wnt9LKqbaWS9nDclFUX9T3JnQgUPsPgIRxiizNbnRoZyfHUgUihRKJ" +
        "UHaDLoAfIZTlHDUrRmiBZBXteQSs3G2Xh_T0zh0D3oKNLRfMhmZFf1x0Oh7Fw7k8WJXeGDzh8-2SV7PtoJgjGI7va4IQ7Wmgg7Sm" +
        "--RfdXDnyq9BtL9Wl2RuQbyzyyAdGuSST3rt6duiLKCbAS_Xl54n08P2AEIv-_qrglcG7tXuZx2aMM8mX3Rzha__7hZ1D41dO6ot" +
        "K84z3BU4LjrYicxYP3qwTzEJul-YOg8AfKa_NnpgWtITmwNklYGsSzk-tR1E1SqvaN-gcKVWm4-LutvXbI8BgmUrbKurePlRgnlZ" +
        "_5m8kmdo9ZbRpXBzqoC0M0jods4nyFOw-hwaD3wDDt0EF1yUGy6yKbEvIboH6qA"


private const val SERVER_ERROR_TOKEN: String = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJ" +
        "hdWQiOiJuaHMtb25saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh" +
        "0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjg" +
        "2LCJhbGciOiJSUzUxMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmOD" +
        "BiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYX" +
        "V0aC5leHQuc2lnbmluLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cn" +
        "VzdG1hcmsvYXV0aC5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbn" +
        "QiOiI5NzQ0MzY2NzUzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMj" +
        "I3OSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIi" +
        "widm90IjoiUDkuQ3AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4NiwicmVhc29uX2Zvcl9yZXF1ZXN0IjoicG" +
        "F0aWVudGFjY2VzcyIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.E6iMyruovf0ZKxjakpNNI" +
        "i4nTB8KdtFQcHGMF6PAQ2MuqXUSk2kn43rQHUSfKZ9D2Jp6ErNmr1h5Y1NkX5N_7CSvT1Vvzy2iQwCx6bEab3pYFiUaMEfvZZvXH" +
        "dql0z2cFYj81T7D_CL6RjkT5XNDy-unF5SbNDB49Cc9oK2rgreu6yvMxbhfXNdbyTsGd80iDNlxUayx-6xUInhbarGlhm5g8NCom" +
        "c0SJoquHN4ooFQN3RCApyuWkG1DFnnk6mFFbckzG_vwErYHKyfFxqXaToFkwhuQIki3U4m5LNam57hvx1BzYILPm5YBvgN2z2wuX" +
        "KU4T9iBUE91-VfweIQr1P2-Kow5PHOPLupYr5db4_49-SRadj045891IyrECZhAHAm7ztcrpHcslZogpCdMMFN4-BNm7mzWLSBcdp" +
        "Xx9tnTomxfEqksiCI7ywBpYxjOmumshPy5TAlWbaIRgTOlXcpL8NiEKz6qrZ7gHNnbyiE5oQ_exXl5ukvVi56FUvPS2vAMp_GJWZU" +
        "OvYINENnMLvFsNdSubNrxkAZucsE23Ld0jAmrPJALxatKvbyJEs6tGsBWBs_4bmJ5yLy36QSpdDElN0Zp4HWjfP0Wy34kovEIn_wB" +
        "qKgfrsQ0wH5RzVODReB5zVOyEd_uVBrlz1pP_T1OS9b6XZi8j9F1GIr49jY"