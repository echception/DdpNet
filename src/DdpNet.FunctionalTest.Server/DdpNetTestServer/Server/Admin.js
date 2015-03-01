Meteor.methods({
    'cleanup': function() {
        Entries.remove({});
    }
});