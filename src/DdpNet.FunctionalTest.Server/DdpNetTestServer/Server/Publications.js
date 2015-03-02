Meteor.publish('entries', function() {
    return Entries.find();
});

Meteor.publish('entriesByName', function(entryName) {
    return Entries.find({
        Name: entryName
    })
});

Meteor.publish('activeEntries', function() {
    return Entries.find({
        IsActive: true
    })
});

Meteor.publish('inactiveEntries', function() {
    return Entries.find({
        IsActive: false
    })
});

Meteor.publish('denyAll', function() {
    return DenyAll.find();
});