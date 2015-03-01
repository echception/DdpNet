Meteor.methods({
    'cleanup': function() {
        Entries.remove({});
        DenyAll.remove({});
    },
    'addDenyAll' : function(denyEntry) {
        DenyAll.insert(denyEntry);
    }
});