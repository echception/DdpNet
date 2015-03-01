Meteor.publish('entries', function() {
    return Entries.find();
});

Meteor.publish('entriesByName', function(entryName) {
    return Entries.find({
        Name: entryName
    })
});