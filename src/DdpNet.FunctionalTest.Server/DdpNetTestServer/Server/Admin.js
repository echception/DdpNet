Meteor.methods({
    'cleanup': function() {
        Entries.remove({});
        DenyAll.remove({});
        Meteor.users.remove({});
    },
    'addDenyAll' : function(denyEntry) {
        DenyAll.insert(denyEntry);
    }
});