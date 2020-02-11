const withSass = require('@zeit/next-sass');
const withFonts = require('next-fonts');
const withBundleAnalyzer = require('@next/bundle-analyzer')({
  enabled: process.env.ANALYZE === 'true'
});
// const isProd = process.env.NODE_ENV === 'production';

module.exports = withBundleAnalyzer(
  withFonts(
    withSass({
      devIndicators: {
        autoPrerender: false
      },
      target: 'serverless',
      // assetPrefix: isProd ? 'http://apps.capsonic.com/UniversalCatalogs/main' : '',
      // exportPathMap: function() {
      //   return {
      //     '/': { page: '/' },
      //     '/items': { page: '/items' }
      //   };
      // },
      analyzeServer: ['server', 'both'].includes(process.env.BUNDLE_ANALYZE),
      analyzeBrowser: ['browser', 'both'].includes(process.env.BUNDLE_ANALYZE),
      bundleAnalyzerConfig: {
        server: {
          analyzerMode: 'static',
          reportFilename: '../bundles/server.html'
        },
        browser: {
          analyserMode: 'static',
          reportFilename: '../bundles/client.html'
        }
      }
    })
  )
);
