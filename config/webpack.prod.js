const common = require("./webpack.common");
const CopyWebpackPlugin = require('copy-webpack-plugin');
const PrerenderSPAPlugin = require("prerender-spa-plugin");
const ResourceHintWebpackPlugin = require('resource-hints-webpack-plugin');

console.log("Bundling for production...");

module.exports = {
  mode: "production",
  entry: common.config.entry,
  output: {
    filename: '[name].[hash].js',
    // path: common.config.buildDir,
    publicPath: '/',
  },
  module: {
    rules: common.getModuleRules(true)
  },
  plugins: common.getPlugins(true).concat([
    new CopyWebpackPlugin([ { from: common.config.publicDir } ]),
    new PrerenderSPAPlugin({
      // The path to the folder where index.html is.
      staticDir: common.config.buildDir,
      // List of routes to prerender.
      routes: [
        '/',
        '/about',
      ],
      minify: {
        collapseWhitespace: true,
      },
    }),
    new ResourceHintWebpackPlugin(),
  ]),
  resolve: {
    modules: [common.config.nodeModulesDir]
  },
};
