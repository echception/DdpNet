Entries = new Meteor.Collection('entries');

Entries.allow({
    insert: function(userId, doc) {
        return true;
    },
    update: function(userId, doc) {
        return true;
    },
    remove: function(userId, doc) {
        console.log('remove');
        return true;
    }
});