'use strict';

var utils = require('../utils/writer.js');
var Pets = require('../service/PetsService');

module.exports.createPets = function createPets (req, res, next) {
  Pets.createPets()
    .then(function (response) {
      utils.writeJson(res, response);
    })
    .catch(function (error_) {
      utils.writeJson(res, error_);
    });
};

module.exports.listPets = function listPets (req, res, next, limit) {
  Pets.listPets(limit)
    .then(function (response) {
      utils.writeJson(res, response);
    })
    .catch(function (error_) {
      utils.writeJson(res, error_);
    });
};

module.exports.showPetById = function showPetById (req, res, next, petId) {
  Pets.showPetById(petId)
    .then(function (response) {
      utils.writeJson(res, response);
    })
    .catch(function (error_) {
      utils.writeJson(res, error_);
    });
};
