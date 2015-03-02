Meteor.methods({
    'returnTen' : function() {
        return 10;
    },
    'throwException': function() {
        throw "ERROR";
    },
    'increment': function(value) {
        return value + 1;
    }
});