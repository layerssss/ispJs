var $ = function (str) {
    $_native_writer_write(String(str));
};
var $load = function (path) {
    path = String(path);
    $_native_loadedJS += '|' + path;
    return new Function("try {for (key in arguments[0]) {this[key] = arguments[0][key];}}catch (e) {}"+$_native_loadJS(path));
};
