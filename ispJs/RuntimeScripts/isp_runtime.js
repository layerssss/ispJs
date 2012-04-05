var $= function (str) {
    $_native_writer_write(String(str));
};
var $load = function (path) {
    path = String(path);
    $_native_loadedJS += '|' + path;
    return new Function($_native_loadJS(path));
};