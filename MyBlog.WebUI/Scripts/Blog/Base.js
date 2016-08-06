CheckEmail = function(email) {
    if (email.length<=0) {
        return false;
    }
    var reg = /[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?/;
    return reg.test(email);
};
CheckUName = function (uname) {
    if (uname.length <= 0) {
        return false;
    }
    var reg = /^[a-zA-Z][a-zA-Z0-9]{3,11}$/;
    return reg.test(uname);
}
CheckPassword = function (pwd) {
    if (pwd.length <= 0) {
        return false;
    }
    var reg = /^[a-zA-Z]\w{5,15}$/;
    return reg.test(pwd);
}
CheckUNickName= function(unickname) {
    if (unickname.length<=0) {
        return false;
    }
    var reg = /^[a-zA-Z\u4e00-\u9fa5][a-zA-Z0-9_\u4E00-\u9FA5]{1,11}$/;
    return reg.test(unickname);
}
