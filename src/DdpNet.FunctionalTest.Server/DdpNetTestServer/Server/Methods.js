Meteor.methods({
    'returnTen' : function() {
        return 10;
    },
    'throwException': function() {
        throw new Meteor.Error("ERROR");
    },
    'increment': function(value) {
        return value + 1;
    }
});