package mocking.stubs

import config.Config
import models.patients.EmisPatients
import models.Patient
import worker.models.session.UserSessionRequest

class EmisStubsPatientFactory {
    companion object {
        private val CONNECTION_TOKEN_SUFFIX_LENGTH = 12
        private val CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH = 2

        val goodToken = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQiOiJuaHMtb25" +
                "saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHBzOi8vYXV0aC5" +
                "leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjgyLCJhbGciOiJSUzU" +
                "xMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZT" +
                "QtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbm" +
                "luLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hcmsvYXV0aC" +
                "5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI5NzQ0MzY2Nz" +
                "UzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OSwic2NvcGUiOi" +
                "JvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm90IjoiUDkuQ3" +
                "AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4MiwicmVhc29uX2Zvcl9yZXF1ZXN0IjoicGF0aWVudGFjY2Vzcy" +
                "IsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.n9F1ovjlxNDSqGob7_yrBEYsEID08J4s9eLSj" +
                "CMYQS_TZO63RHWOlnjG8WrZcDPfH5W8yuMw5XBYwbAWD8DasMXy2Fe2z65LP6eL2-SnutQwWl4InLVu8saIzRhX3L2PUy5kLJNap" +
                "JacbvSUioJm48ClmJff6T9UABfCMB3N2O-QzLUoH6drkfJbwblZRB8_mV4px768gdAD1Yg-hMW25UgnJ2ExjQPMDP1-lM0aNF_Ie" +
                "-ejG84F8xabLEJ6ZO3Y_-8x7JZgMF0P-4-4g3RMCNd4xWH1QOZ2jX2CiOZ8Mtv_QU7ia_KEJh2yzZyEFbEdYaEO2TpSCnxwdDjMw" +
                "X_RGYN3NDf-5ZymYpf6Vm8PSxRbmT28xGNSH856DnlM8c4cvZ9Nn3rBFt0lJRDIutQfgL63mNk7_VQYY3uXADNoJFJwyvZMS9rFW" +
                "utzjQWV-uRlND3z1YQNsZ6s0Ha4d6dtvDOE6zdyM8soHpiYiuvo9GtLCg9984EYKYP69sHvwLtxb5txpk5HqemowZvF76sJIJbl4" +
                "L8QfMHZ4lpsIKpBY_s4qNYGczb3aUm23coJD0v31_P1peRO67NaFl0_ie7xbosnPVFnPt0p29E9NbORLcl1bB-S9YGokAPIRyAeH" +
                "khvu0-w89dkAIXBTWf1vFJeCg7X2MNG7Y_LW8EgtPOLW1s"

        var timeoutToken = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQiOiJuaHMtb25" +
                "saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHBzOi8vYXV0aC5" +
                "leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjgzLCJhbGciOiJSUzU" +
                "xMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZT" +
                "QtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbm" +
                "luLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hcmsvYXV0aC" +
                "5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI5NzQ0MzY2Nz" +
                "UzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OSwic2NvcGUiOi" +
                "JvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm90IjoiUDkuQ3" +
                "AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4MywicmVhc29uX2Zvcl9yZXF1ZXN0IjoicGF0aWVudGFjY2Vzcy" +
                "IsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.u6LUd1VIKf5ivvKvs3OoG3LY1ip7DNwyUH9xo" +
                "rfoSXVXQSatoqbGDIf5ohEUotALMjI7Hd7gAXAIbeBbUT8XDZtOccJ_tgHllWdM1IlQN7Aw72hjHqqHvA_v2Wx5Wj0mHJXxS2on5" +
                "jkU47vJAlwlHrlX5scB7b923-XO8By7b6OC1VFvnHs_SaeYTsix-BcO3BWxN7Rsfaf5mGGKXpkPzEi_qLSdXsZ0fwuGeHxAQV417" +
                "nbNHwCW8ZHxTBQKm1Nd5-uWlnBM7CfrN6bkzDnTxuh1TjL8hUX_1liIDgm9b-IOz-9KWpWdEY7qFWxMS2g-4YuGppohdFlrDmlUM" +
                "bpNwDYe_YAsZPJZzNpNF_SGLUKfgjzfbC59FBe3Nd-2qa0TIUqorZ6VJNKwNBhxh8HBWGxrPS2yaZ5Ly5FNlStjNruMoI5kEpQV0" +
                "kHA0FR-KyhFGPuumCNc7uZF9wq_BNJGdtyrRMV8t0_iZqZqaEZf49Im1ND2rqetTCGoZR1RhQ8D63Jlb-8OJSJIUgq00UJqhqdbE" +
                "kLmx3SB9tX3XMREh27B7jgl5FY6glIQIUD_uEmreB36dLihpythu_7r8BUwOV4gcYxy5W0Uvcn_xEDc3YrWDuEr_5-4oCdQfK0Zg" +
                "Stt9BegAX4qGq6_FCVcp81tztMwoG3YM40rUfFmIAjc0hs"

        val serviceNotEnabledToken = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQiOiJuaHMtb25" +
                "saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHBzOi8vYXV0aC5" +
                "leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjg0LCJhbGciOiJSUzU" +
                "xMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZT" +
                "QtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbm" +
                "luLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hcmsvYXV0aC" +
                "5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI5NzQ0MzY2Nz" +
                "UzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OSwic2NvcGUiOi" +
                "JvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm90IjoiUDkuQ3" +
                "AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4NCwicmVhc29uX2Zvcl9yZXF1ZXN0IjoicGF0aWVudGFjY2Vzcy" +
                "IsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.o4UdpxpdLLQ7bfZXmSoFv99x0eC15cBqVEpHz" +
                "jZ9nQcsOPheUnj6su7RWpp3MxuseM1cePvHuss16Dmw_kH_21s_FgPCSFn1WOPjyS_gdx0cuT3BpZIkcujJ5ZhQ3fPOWslH7583-" +
                "1IgGQ6Y5ElC6mwRvXLMnf0aNJ6i4_RwivEZYurnQSnXG7BsmuXpKyxKlLLoXrixoQMHOoAzqUyrxMiGaHfKaqVfyCjBEYSiFuikT" +
                "mldJAZ5VWZ9uR1rs3om5tN0CDdNcwHzlQvmIS5MQ2pP4yjZH2Hy6HuiTWnFQYzTPWo0s1bXH0z_RoiNreW7is60q5CoxVZV-Eztj" +
                "lkEQJgF6ZR9uFe6NNGf1LXsID2n3GsVWANk_rvSqYuW8CGRuewAHFeyrQildjdpKwoR8rwE2j5Tjue_hefE_BYLRDuCytInfxDw8" +
                "K4pKKaS7gxQwGYKVZZ8jo4I_-j91neQMV3dfE0D2nWe-9R62LLt4g86K7oWJOzbxb5szEUErmPiB1a4IAYVbX8rOWMREM1WOO88l" +
                "tnHwRMdVhL36f0WRRfwjRFIQ0hjQtvmivmor7tlGHMuv9ErTuETskhbHJAkxWbkQW2VxmWxfl-DUdUFVxC-nl5BmwK9Cni6e5fcd" +
                "oHcWX-0CzgOzZ3ZTRLJADYLRo7RJec4QNWuyf2fnvtV3Gc"

        val sessionErrorToken = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQiOiJuaHMtb25" +
                "saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHBzOi8vYXV0aC5" +
                "leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjg1LCJhbGciOiJSUzU" +
                "xMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZT" +
                "QtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbm" +
                "luLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hcmsvYXV0aC" +
                "5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI5NzQ0MzY2Nz" +
                "UzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OSwic2NvcGUiOi" +
                "JvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm90IjoiUDkuQ3" +
                "AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4NSwicmVhc29uX2Zvcl9yZXF1ZXN0IjoicGF0aWVudGFjY2Vzcy" +
                "IsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.SReVcOvo6RuNk5_MJB5_NkoYMgmENU8FJpq0t" +
                "8y4-LLHZpncpyNLoF-Obq7AnOBbty6lX6Plk0tVfLStkjunWArK9RMBIuOF0TWg42RejZZHe2KYklOsSq52-RPtDU_JOpy2gw037" +
                "_Bq8C5JJPuyPPCz7qtNVAKYSMggarPX6Me6WjQBm51fVC2LnCiC5TFlvzrS2Yg6iwZ-cgEu3iXcPWnhtdP2NWclIdeSZQAqvHAyZ" +
                "oHGrMFYm5NWpF64r-Mx23gSJmO5e9Wnt9LKqbaWS9nDclFUX9T3JnQgUPsPgIRxiizNbnRoZyfHUgUihRKJUHaDLoAfIZTlHDUrR" +
                "miBZBXteQSs3G2Xh_T0zh0D3oKNLRfMhmZFf1x0Oh7Fw7k8WJXeGDzh8-2SV7PtoJgjGI7va4IQ7Wmgg7Sm--RfdXDnyq9BtL9Wl" +
                "2RuQbyzyyAdGuSST3rt6duiLKCbAS_Xl54n08P2AEIv-_qrglcG7tXuZx2aMM8mX3Rzha__7hZ1D41dO6otK84z3BU4LjrYicxYP" +
                "3qwTzEJul-YOg8AfKa_NnpgWtITmwNklYGsSzk-tR1E1SqvaN-gcKVWm4-LutvXbI8BgmUrbKurePlRgnlZ_5m8kmdo9ZbRpXBzq" +
                "oC0M0jods4nyFOw-hwaD3wDDt0EF1yUGy6yKbEvIboH6qA"

        val serverErrorToken = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQiOiJuaHMtb25" +
                "saW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHBzOi8vYXV0aC5" +
                "leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjg2LCJhbGciOiJSUzU" +
                "xMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZT" +
                "QtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbm" +
                "luLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hcmsvYXV0aC" +
                "5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI5NzQ0MzY2Nz" +
                "UzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OSwic2NvcGUiOi" +
                "JvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm90IjoiUDkuQ3" +
                "AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4NiwicmVhc29uX2Zvcl9yZXF1ZXN0IjoicGF0aWVudGFjY2Vzcy" +
                "IsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.E6iMyruovf0ZKxjakpNNIi4nTB8KdtFQcHGMF" +
                "6PAQ2MuqXUSk2kn43rQHUSfKZ9D2Jp6ErNmr1h5Y1NkX5N_7CSvT1Vvzy2iQwCx6bEab3pYFiUaMEfvZZvXHdql0z2cFYj81T7D_" +
                "CL6RjkT5XNDy-unF5SbNDB49Cc9oK2rgreu6yvMxbhfXNdbyTsGd80iDNlxUayx-6xUInhbarGlhm5g8NComc0SJoquHN4ooFQN3" +
                "RCApyuWkG1DFnnk6mFFbckzG_vwErYHKyfFxqXaToFkwhuQIki3U4m5LNam57hvx1BzYILPm5YBvgN2z2wuXKU4T9iBUE91-Vfwe" +
                "IQr1P2-Kow5PHOPLupYr5db4_49-SRadj045891IyrECZhAHAm7ztcrpHcslZogpCdMMFN4-BNm7mzWLSBcdpXx9tnTomxfEqksi" +
                "CI7ywBpYxjOmumshPy5TAlWbaIRgTOlXcpL8NiEKz6qrZ7gHNnbyiE5oQ_exXl5ukvVi56FUvPS2vAMp_GJWZUOvYINENnMLvFsN" +
                "dSubNrxkAZucsE23Ld0jAmrPJALxatKvbyJEs6tGsBWBs_4bmJ5yLy36QSpdDElN0Zp4HWjfP0Wy34kovEIn_wBqKgfrsQ0wH5Rz" +
                "VODReB5zVOyEd_uVBrlz1pP_T1OS9b6XZi8j9F1GIr49jY"

        val goodPatientEMIS = generatePatientData(
                "1", "goodpatient", CONNECTION_TOKEN_SUFFIX_LENGTH, goodToken)
        val timeoutPatientEMIS = generatePatientData(
                "2", "timeoutpatient", CONNECTION_TOKEN_SUFFIX_LENGTH, timeoutToken)
        val serviceNotEnabledPatientEMIS = generatePatientData(
                "3", "servicennotenabledpatient", CONNECTION_TOKEN_SUFFIX_LENGTH, serviceNotEnabledToken)
        val sessionErrorPatientEMIS = generatePatientData(
                "4", "sessionerrorpatient", CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH, sessionErrorToken)
        val serverErrorPatientEMIS = generatePatientData(
                "5", "servererrorpatient", CONNECTION_TOKEN_SUFFIX_LENGTH, serverErrorToken)

        val EMISPatientList = listOf(
                goodPatientEMIS,
                serviceNotEnabledPatientEMIS,
                sessionErrorPatientEMIS,
                timeoutPatientEMIS,
                serverErrorPatientEMIS
        )

        private fun generatePatientData(uniqueId: String, loginID: String, length: Int, accessToken: String):
                Patient {
            val pad = uniqueId.padStart(length, '0')
            //do not add end user session id here

            return EmisPatients.picaJones.copy(
                    firstName = "You are logged in as",
                    surname = loginID,
                    cidUserSession = UserSessionRequest(
                            authCode = "authCode$pad",
                            codeVerifier = "codeVerifier$pad",
                            redirectUrl = Config.instance.cidRedirectUri
                    ),
                    accessToken = accessToken,
                    connectionToken = "00000000-0000-0000-0000-$pad",
                    userPatientLinkToken = "userPatientLinkToken$pad"
            )
        }
    }
}