Meteor.methods({
    'cleanup': function() {
        console.log('cleanup');
        Entries.remove({});
    }
});