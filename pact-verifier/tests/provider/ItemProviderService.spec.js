const { Verifier } = require('@pact-foundation/pact');
const { describe, it } = require('mocha');
const path = require('path');
const { createStateHandlers } = require('./providerStates');

const pactFile = path.resolve('./pacts/react-client-inventory-service.json');
const providerBaseUrl = 'http://localhost:5094';

const opts = {
    provider: "inventory-service",
    providerBaseUrl,
    logLevel: "info",
    pactUrls: [pactFile],
    stateHandlers: createStateHandlers(providerBaseUrl),
};

describe("Pact Verification", () => {
    it("Valida lo que espera el API del Cliente", () => {
        return new Verifier(opts).verifyProvider();
    });
});
