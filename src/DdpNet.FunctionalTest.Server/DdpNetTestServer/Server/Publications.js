Meteor.publish('entries', function() {
    return Entries.find();
});